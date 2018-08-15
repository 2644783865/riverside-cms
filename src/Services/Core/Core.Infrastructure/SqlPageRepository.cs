using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Core.Domain;
using static Dapper.SqlMapper;

namespace Riverside.Cms.Services.Core.Infrastructure
{
    public class SqlPageRepository : IPageRepository
    {
        private readonly IOptions<SqlOptions> _options;

        private const string TaggedPagesTableName = "TaggedPages";

        public SqlPageRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<Page> ReadPageAsync(long tenantId, long pageId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                using (GridReader gr = await connection.QueryMultipleAsync(@"
                    SELECT
                        TenantId,
                        PageId,
                        ParentPageId,
                        Name,
                        Description,
                        Created,
                        Updated,
                        Occurred,
                        MasterPageId,
                        ImageUploadId AS ImageBlobId,
                        PreviewImageUploadId AS PreviewImageBlobId,
                        ThumbnailImageUploadId AS ThumbnailImageBlobId
                    FROM
                        cms.Page
                    WHERE
                        TenantId = @TenantId AND
                        PageId = @PageId

                    SELECT
                        cms.Tag.TagId,
                        cms.Tag.Name
                    FROM
                        cms.Tag
                    INNER JOIN
                        cms.TagPage
                    ON
                        cms.Tag.TenantId = cms.TagPage.TenantId AND
                        cms.Tag.TagId = cms.TagPage.TagId
                    WHERE
                        cms.TagPage.TenantId = @TenantId AND
                        cms.TagPage.PageId = @PageId
                    ORDER BY
                        cms.Tag.Name",
                    new { TenantId = tenantId, PageId = pageId }))
                {
                    Page page = await gr.ReadFirstOrDefaultAsync<Page>();
                    if (page != null)
                        page.Tags = await gr.ReadAsync<Tag>();
                    return page;
                }
            }
        }

        private IEnumerable<Page> PopulatePageTags(IEnumerable<Page> pages, IEnumerable<PageTag> pageTags)
        {
            foreach (Page page in pages)
            {
                page.Tags = pageTags.Where(pt => pt.PageId == page.PageId).Select(pt => new Tag { TagId = pt.TagId, Name = pt.Name });
                yield return page;
            }
        }

        public async Task<IEnumerable<Page>> ListPagesInHierarchyAsync(long tenantId, long pageId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                using (GridReader gr = await connection.QueryMultipleAsync(@"
                    DECLARE @Pages TABLE ([Level] [int] NOT NULL PRIMARY KEY CLUSTERED, [PageId] [bigint] NOT NULL)

                    ;WITH [Pages] AS
                    (
	                    SELECT
		                    0 AS [Level],
		                    cms.[Page].PageId,
		                    cms.[Page].ParentPageId
	                    FROM
		                    cms.[Page]
	                    WHERE
		                    cms.[Page].TenantId = @TenantId AND
		                    cms.[Page].PageId = @PageId
	                    UNION ALL
	                    SELECT
		                    [Pages].[Level] + 1 AS [Level],
		                    ParentPage.PageId,
		                    ParentPage.ParentPageId
	                    FROM
		                    cms.[Page] ParentPage
	                    INNER JOIN
		                    [Pages]
	                    ON
		                    ParentPage.TenantId = @TenantId AND
		                    ParentPage.PageId = [Pages].ParentPageId
                    )

                    INSERT INTO
                        @Pages (Level, PageId)
                    SELECT
                        [Pages].Level,
                        [Pages].PageId
                    FROM
                        [Pages]
                    ORDER BY
                        [Pages].[Level] ASC

                    SELECT
	                    cms.[Page].TenantId,
	                    cms.[Page].PageId,
	                    cms.[Page].ParentPageId,
	                    cms.[Page].MasterPageId,
	                    cms.[Page].Name,
	                    cms.[Page].[Description],
	                    cms.[Page].Created,
	                    cms.[Page].Updated,
	                    cms.[Page].Occurred,
	                    cms.[Page].ThumbnailImageUploadId AS ThumbnailImageBlobId,
	                    cms.[Page].PreviewImageUploadId AS PreviewImageBlobId,
	                    cms.[Page].ImageUploadId AS ImageBlobId
                    FROM
	                    cms.[Page]
                    INNER JOIN
	                    @Pages Pages
                    ON
	                    cms.[Page].TenantId = @TenantId AND
	                    cms.[Page].PageId = Pages.PageId
                    ORDER BY
	                    Pages.Level ASC

                    SELECT
                        cms.TagPage.PageId,
                        cms.Tag.TagId,
                        cms.Tag.Name
                    FROM
                        cms.Tag
                    INNER JOIN
                        cms.TagPage
                    ON
                        cms.Tag.TenantId = cms.TagPage.TenantId AND
                        cms.Tag.TagId = cms.TagPage.TagId
                    INNER JOIN
                        @Pages Pages
                    ON
                        cms.TagPage.TenantId = @TenantId AND
                        cms.TagPage.PageId = Pages.PageId
                    ORDER BY
                        cms.TagPage.PageId,
                        cms.Tag.Name
                ",
                new
                {
                    TenantId = tenantId,
                    PageId = pageId
                }))
                {
                    IEnumerable<Page> pages = await gr.ReadAsync<Page>();
                    IEnumerable<PageTag> pageTabs = await gr.ReadAsync<PageTag>();
                    return PopulatePageTags(pages, pageTabs);
                }
            }
        }

        private string GetListPagesCteSql()
        {
            return @"
                ;WITH [Pages] AS
                (
                    SELECT TOP (@RowNumberUpperBound)
                        ROW_NUMBER() OVER (ORDER BY
			                CASE WHEN @SortBy = 0 /* PageSortBy.Created  */ AND @SortAsc = 0 THEN cms.[Page].Created  END DESC,
			                CASE WHEN @SortBy = 1 /* PageSortBy.Updated  */ AND @SortAsc = 0 THEN cms.[Page].Updated  END DESC,
			                CASE WHEN @SortBy = 2 /* PageSortBy.Occurred */ AND @SortAsc = 0 THEN cms.[Page].Occurred END DESC,
			                CASE WHEN @SortBy = 3 /* PageSortBy.Name     */ AND @SortAsc = 0 THEN cms.[Page].Name     END DESC,
			                CASE WHEN @SortBy = 0 /* PageSortBy.Created  */ AND @SortAsc = 1 THEN cms.[Page].Created  END ASC,
			                CASE WHEN @SortBy = 1 /* PageSortBy.Updated  */ AND @SortAsc = 1 THEN cms.[Page].Updated  END ASC,
			                CASE WHEN @SortBy = 2 /* PageSortBy.Occurred */ AND @SortAsc = 1 THEN cms.[Page].Occurred END ASC,
			                CASE WHEN @SortBy = 3 /* PageSortBy.Name     */ AND @SortAsc = 1 THEN cms.[Page].Name     END ASC) AS RowNumber,
		                cms.[Page].PageId
                    FROM
                        cms.[Page]
	                INNER JOIN
		                cms.[MasterPage]
	                ON
		                cms.[Page].TenantId = cms.MasterPage.TenantId AND
		                cms.[Page].MasterPageId = cms.MasterPage.MasterPageId
                    WHERE
		                cms.[Page].TenantId = @TenantId AND
                        cms.[Page].ParentPageId = @ParentPageId AND
		                cms.MasterPage.PageType = @PageType
                )
            ";
        }

        private string GetListPagesCteRecursiveSql()
        {
            return @"
                -- Get pages into order so that we can extract the right pages according to the paging and sorting parameters

                ;WITH [Pages] AS
                (
                    SELECT TOP (@RowNumberUpperBound)
                        ROW_NUMBER() OVER (ORDER BY
			                CASE WHEN @SortBy = 0 /* PageSortBy.Created  */ AND @SortAsc = 0 THEN cms.[Page].Created  END DESC,
			                CASE WHEN @SortBy = 1 /* PageSortBy.Updated  */ AND @SortAsc = 0 THEN cms.[Page].Updated  END DESC,
			                CASE WHEN @SortBy = 2 /* PageSortBy.Occurred */ AND @SortAsc = 0 THEN cms.[Page].Occurred END DESC,
			                CASE WHEN @SortBy = 3 /* PageSortBy.Name     */ AND @SortAsc = 0 THEN cms.[Page].Name     END DESC,
			                CASE WHEN @SortBy = 0 /* PageSortBy.Created  */ AND @SortAsc = 1 THEN cms.[Page].Created  END ASC,
			                CASE WHEN @SortBy = 1 /* PageSortBy.Updated  */ AND @SortAsc = 1 THEN cms.[Page].Updated  END ASC,
			                CASE WHEN @SortBy = 2 /* PageSortBy.Occurred */ AND @SortAsc = 1 THEN cms.[Page].Occurred END ASC,
			                CASE WHEN @SortBy = 3 /* PageSortBy.Name     */ AND @SortAsc = 1 THEN cms.[Page].Name     END ASC) AS RowNumber,
		                cms.[Page].PageId
                    FROM
                        @Folders [FoldersTable]
	                INNER JOIN
		                cms.[Page]
	                ON
		                cms.[Page].TenantId = @TenantId AND
                        cms.[Page].ParentPageId = [FoldersTable].PageId
	                INNER JOIN
		                cms.[MasterPage]
	                ON
		                cms.[Page].TenantId = cms.MasterPage.TenantId AND
		                cms.[Page].MasterPageId = cms.MasterPage.MasterPageId
                    WHERE
		                cms.MasterPage.PageType	= @PageType
                )
            ";
        }

        private string GetListPagesTotalSql()
        {
            return @"
                SELECT
	                COUNT(*) AS Total
                FROM
	                cms.[Page]
                INNER JOIN
	                cms.[MasterPage]
                ON
	                cms.[Page].TenantId = cms.MasterPage.TenantId AND
	                cms.[Page].MasterPageId = cms.MasterPage.MasterPageId
                WHERE
	                cms.[Page].TenantId = @TenantId AND
                    cms.[Page].ParentPageId = @ParentPageId AND
	                cms.MasterPage.PageType = @PageType
            ";
        }

        private string GetListPagesTotalRecursiveSql()
        {
            return @"
                SELECT
	                COUNT(*) AS Total
                FROM
	                cms.[Page]
                INNER JOIN
	                @Folders [FoldersTable]
                ON
	                cms.[Page].TenantId = @TenantId AND
                    cms.[Page].ParentPageId = [FoldersTable].PageId
                INNER JOIN
	                cms.[MasterPage]
                ON
	                cms.[Page].TenantId = cms.MasterPage.TenantId AND
	                cms.[Page].MasterPageId = cms.MasterPage.MasterPageId
                WHERE
	                cms.MasterPage.PageType	= @PageType
            ";
        }

        private string GetListPagesSetupSql()
        {
            return @"
                DECLARE @Pages TABLE ([RowNumber] [int] NOT NULL PRIMARY KEY CLUSTERED, [PageId] [bigint] NOT NULL)
                DECLARE @RowNumberLowerBound int
                DECLARE @RowNumberUpperBound int
                SET @RowNumberLowerBound = @PageSize * @PageIndex
                SET @RowNumberUpperBound = @RowNumberLowerBound + @PageSize + 1;

                IF (@ParentPageId IS NULL)
	                SET @ParentPageId = (SELECT PageId FROM cms.[Page] WHERE cms.[Page].TenantId = @TenantId AND cms.[Page].ParentPageId IS NULL)
            ";
        }

        private string GetListPagesSelectSql()
        {
            return @"
                INSERT INTO
                    @Pages (RowNumber, PageId)
                SELECT
                    [Pages].RowNumber,
                    [Pages].PageId
                FROM
                    [Pages]
                WHERE
	                [Pages].RowNumber > @RowNumberLowerBound AND [Pages].RowNumber < @RowNumberUpperBound
                ORDER BY
                    [Pages].RowNumber ASC

                SELECT
	                cms.[Page].TenantId,
	                cms.[Page].PageId,
	                cms.[Page].ParentPageId,
	                cms.[Page].MasterPageId,
	                cms.[Page].Name,
	                cms.[Page].[Description],
	                cms.[Page].Created,
	                cms.[Page].Updated,
	                cms.[Page].Occurred,
	                cms.[Page].ImageTenantId,
	                cms.[Page].ThumbnailImageUploadId AS ThumbnailImageBlobId,
	                cms.[Page].PreviewImageUploadId AS PreviewImageBlobId,
	                cms.[Page].ImageUploadId AS ImageBlobId
                FROM
	                @Pages Pages
                INNER JOIN
	                cms.[Page]
                ON
	                cms.[Page].TenantId = @TenantId AND
	                cms.[Page].PageId = Pages.PageId
                ORDER BY
                    Pages.RowNumber ASC

                SELECT
                    cms.TagPage.PageId,
                    cms.Tag.TagId,
                    cms.Tag.Name
                FROM
                    cms.Tag
                INNER JOIN
                    cms.TagPage
                ON
                    cms.Tag.TenantId = cms.TagPage.TenantId AND
                    cms.Tag.TagId = cms.TagPage.TagId
                INNER JOIN
                    @Pages Pages
                ON
                    cms.TagPage.TenantId = @TenantId AND
                    cms.TagPage.PageId = Pages.PageId
                ORDER BY
                    cms.TagPage.PageId,
                    cms.Tag.Name
            ";
        }

        private string GetListPagesSql()
        {
            return $@"
                {GetListPagesSetupSql()}
                {GetListPagesCteSql()}
                {GetListPagesSelectSql()}
                {GetListPagesTotalSql()}
            ";
        }

        private string GetListPagesRecursiveSql()
        {
            return $@"
                {GetListPagesSetupSql()}
                {SqlProvider.GetFoldersRecursiveSql()}
                {GetListPagesCteRecursiveSql()}
                {GetListPagesSelectSql()}
                {GetListPagesTotalRecursiveSql()}
            ";
        }

        private string GetListPagesTaggedPagesCteSql()
        {
            return @"
                ;WITH [Pages] AS
                (
                    SELECT TOP (@RowNumberUpperBound)
                        ROW_NUMBER() OVER (ORDER BY
			                CASE WHEN @SortBy = 0 /* PageSortBy.Created  */ AND @SortAsc = 0 THEN cms.[Page].Created  END DESC,
			                CASE WHEN @SortBy = 1 /* PageSortBy.Updated  */ AND @SortAsc = 0 THEN cms.[Page].Updated  END DESC,
			                CASE WHEN @SortBy = 2 /* PageSortBy.Occurred */ AND @SortAsc = 0 THEN cms.[Page].Occurred END DESC,
			                CASE WHEN @SortBy = 3 /* PageSortBy.Name     */ AND @SortAsc = 0 THEN cms.[Page].Name     END DESC,
			                CASE WHEN @SortBy = 0 /* PageSortBy.Created  */ AND @SortAsc = 1 THEN cms.[Page].Created  END ASC,
			                CASE WHEN @SortBy = 1 /* PageSortBy.Updated  */ AND @SortAsc = 1 THEN cms.[Page].Updated  END ASC,
			                CASE WHEN @SortBy = 2 /* PageSortBy.Occurred */ AND @SortAsc = 1 THEN cms.[Page].Occurred END ASC,
			                CASE WHEN @SortBy = 3 /* PageSortBy.Name     */ AND @SortAsc = 1 THEN cms.[Page].Name     END ASC) AS RowNumber,
                        cms.[Page].TenantId,
		                cms.[Page].PageId
                    FROM
                        @TaggedPages [TaggedPages]
	                INNER JOIN
		                cms.[Page]
	                ON
		                cms.[Page].TenantId = @TenantId AND
		                cms.[Page].PageId = [TaggedPages].PageId
	                INNER JOIN
		                cms.[MasterPage]
	                ON
		                cms.[Page].TenantId = cms.MasterPage.TenantId AND
		                cms.[Page].MasterPageId = cms.MasterPage.MasterPageId
                    WHERE
		                cms.MasterPage.PageType	= @PageType
                )
            ";
        }

        private string GetListPagesTaggedPagesTotalSql()
        {
            return @"
                SELECT
	                COUNT(*) AS Total
                FROM
	                cms.[Page]
                INNER JOIN
	                @TaggedPages [TaggedPages]
                ON
	                cms.[Page].TenantId = @TenantId AND
	                cms.[Page].PageId = [TaggedPages].PageId
                INNER JOIN
	                cms.[MasterPage]
                ON
	                cms.[Page].TenantId = cms.MasterPage.TenantId AND
	                cms.[Page].MasterPageId = cms.MasterPage.MasterPageId
                WHERE
	                cms.MasterPage.PageType	= @PageType
            ";
        }

        private string GetListTaggedPagesSql()
        {
            return $@"
                {GetListPagesSetupSql()}
                {SqlProvider.GetTagsSql()}
                {SqlProvider.GetTaggedPagesSql(TaggedPagesTableName)}
                {GetListPagesTaggedPagesCteSql()}
                {GetListPagesSelectSql()}
                {GetListPagesTaggedPagesTotalSql()}
            ";
        }

        private string GetListPagesTaggedPagesCteRecursiveSql()
        {
            return @"
                -- Get pages into order so that we can extract the right pages according to the paging and sorting parameters

                ;WITH [Pages] AS
                (
                    SELECT TOP (@RowNumberUpperBound)
                        ROW_NUMBER() OVER (ORDER BY
			                CASE WHEN @SortBy = 0 /* PageSortBy.Created  */ AND @SortAsc = 0 THEN cms.[Page].Created  END DESC,
			                CASE WHEN @SortBy = 1 /* PageSortBy.Updated  */ AND @SortAsc = 0 THEN cms.[Page].Updated  END DESC,
			                CASE WHEN @SortBy = 2 /* PageSortBy.Occurred */ AND @SortAsc = 0 THEN cms.[Page].Occurred END DESC,
			                CASE WHEN @SortBy = 3 /* PageSortBy.Name     */ AND @SortAsc = 0 THEN cms.[Page].Name     END DESC,
			                CASE WHEN @SortBy = 0 /* PageSortBy.Created  */ AND @SortAsc = 1 THEN cms.[Page].Created  END ASC,
			                CASE WHEN @SortBy = 1 /* PageSortBy.Updated  */ AND @SortAsc = 1 THEN cms.[Page].Updated  END ASC,
			                CASE WHEN @SortBy = 2 /* PageSortBy.Occurred */ AND @SortAsc = 1 THEN cms.[Page].Occurred END ASC,
			                CASE WHEN @SortBy = 3 /* PageSortBy.Name     */ AND @SortAsc = 1 THEN cms.[Page].Name     END ASC) AS RowNumber,
                        cms.[Page].TenantId,
		                cms.[Page].PageId
                    FROM
                        @TaggedPages [TaggedPages]
	                INNER JOIN
		                cms.[Page]
	                ON
		                cms.[Page].TenantId = @TenantId AND
		                cms.[Page].PageId = [TaggedPages].PageId
	                INNER JOIN
		                cms.[MasterPage]
	                ON
		                cms.[Page].TenantId = cms.MasterPage.TenantId AND
		                cms.[Page].MasterPageId = cms.MasterPage.MasterPageId
                    WHERE
		                cms.MasterPage.PageType	= @PageType
                )
            ";
        }

        private string GetListPagesTaggedPagesTotalRecursiveSql()
        {
            return @"
                SELECT
	                COUNT(*) AS Total
                FROM
	                cms.[Page]
                INNER JOIN
	                @TaggedPages [TaggedPages]
                ON
	                cms.[Page].TenantId = @TenantId AND
	                cms.[Page].PageId = [TaggedPages].PageId
                INNER JOIN
	                cms.[MasterPage]
                ON
	                cms.[Page].TenantId = cms.MasterPage.TenantId AND
	                cms.[Page].MasterPageId = cms.MasterPage.MasterPageId
                WHERE
	                cms.MasterPage.PageType	= @PageType
            ";
        }

        private string GetListTaggedPagesRecursiveSql()
        {
            return $@"
                {GetListPagesSetupSql()}
                {SqlProvider.GetFoldersRecursiveSql()}
                {SqlProvider.GetTagsSql()}
                {SqlProvider.GetTaggedPagesRecursiveSql(TaggedPagesTableName)}
                {GetListPagesTaggedPagesCteRecursiveSql()}
                {GetListPagesSelectSql()}
                {GetListPagesTaggedPagesTotalRecursiveSql()}
            ";
        }

        private string GetListPagesSql(bool recursive, IEnumerable<long> tagIds)
        {
            string sql = null;
            bool filterByTags = tagIds != null && tagIds.Count() > 0;
            if (filterByTags)
            {
                if (recursive)
                    sql = GetListTaggedPagesRecursiveSql();
                else
                    sql = GetListTaggedPagesSql();
            }
            else
            {
                if (recursive)
                    sql = GetListPagesRecursiveSql();
                else
                    sql = GetListPagesSql();
            }
            return sql;
        }

        public async Task<PageListResult> ListPagesAsync(long tenantId, long? parentPageId, bool recursive, PageType pageType, IEnumerable<long> tagIds, SortBy sortBy, bool sortAsc, int pageIndex, int pageSize)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                using (GridReader gr = await connection.QueryMultipleAsync(
                    GetListPagesSql(recursive, tagIds),
                    new
                    {
                        TenantId = tenantId,
                        ParentPageId = parentPageId,
                        SortBy = sortBy,
                        SortAsc = sortAsc,
                        PageType = pageType,
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        TagIds = tagIds
                    }
                ))
                {
                    IEnumerable<Page> pages = await gr.ReadAsync<Page>();
                    IEnumerable<PageTag> pageTabs = await gr.ReadAsync<PageTag>();
                    int total = await gr.ReadFirstAsync<int>();
                    pages = PopulatePageTags(pages, pageTabs);
                    return new PageListResult
                    {
                        Pages = pages,
                        Total = total
                    };
                }
            }
        }

        public async Task<IEnumerable<PageZone>> SearchPageZonesAsync(long tenantId, long pageId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                IEnumerable<PageZone> pageZones = await connection.QueryAsync<PageZone>(
                    @"SELECT TenantId, PageId, PageZoneId, MasterPageId, MasterPageZoneId
                        FROM cms.PageZone WHERE TenantId = @TenantId AND PageId = @PageId
                        ORDER BY PageZoneId",
                    new { TenantId = tenantId, PageId = pageId }
                );
                return pageZones;
            }
        }

        public async Task<PageZone> ReadPageZoneAsync(long tenantId, long pageId, long pageZoneId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();

                PageZone pageZone = await connection.QueryFirstOrDefaultAsync<PageZone>(
                    @"SELECT TenantId, PageId, PageZoneId, MasterPageId, MasterPageZoneId
                        FROM cms.PageZone WHERE TenantId = @TenantId AND PageId = @PageId AND PageZoneId = @PageZoneId",
                    new { TenantId = tenantId, PageId = pageId, PageZoneId = pageZoneId }
                );

                return pageZone;
            }
        }

        public async Task<IEnumerable<PageZoneElement>> SearchPageZoneElementsAsync(long tenantId, long pageId, long pageZoneId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                IEnumerable<PageZoneElement> pageZoneElements = await connection.QueryAsync<PageZoneElement>(
                    @"SELECT cms.PageZoneElement.TenantId, cms.PageZoneElement.PageId, cms.PageZoneElement.PageZoneId, cms.PageZoneElement.PageZoneElementId, cms.PageZoneElement.SortOrder,
                        cms.Element.ElementTypeId, cms.PageZoneElement.ElementId, cms.PageZoneElement.MasterPageId, cms.PageZoneElement.MasterPageZoneId, cms.PageZoneElement.MasterPageZoneElementId
	                    FROM cms.PageZoneElement INNER JOIN cms.Element ON 
                        cms.PageZoneElement.TenantId = cms.Element.TenantId AND cms.PageZoneElement.ElementId = cms.Element.ElementId
                        WHERE cms.PageZoneElement.TenantId = @TenantId AND cms.PageZoneElement.PageId = @PageId AND cms.PageZoneElement.PageZoneId = @PageZoneId
                        ORDER BY cms.PageZoneElement.SortOrder ASC, cms.PageZoneElement.PageZoneElementId ASC",
                    new { TenantId = tenantId, PageId = pageId, PageZoneId = pageZoneId }
                );
                return pageZoneElements;
            }
        }

        public async Task<PageZoneElement> ReadPageZoneElementAsync(long tenantId, long pageId, long pageZoneId, long pageZoneElementId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();

                PageZoneElement pageZoneElement = await connection.QueryFirstOrDefaultAsync<PageZoneElement>(
                    @"SELECT cms.PageZoneElement.TenantId, cms.PageZoneElement.PageId, cms.PageZoneElement.PageZoneId, cms.PageZoneElement.PageZoneElementId, cms.PageZoneElement.SortOrder,
                        cms.Element.ElementTypeId, cms.PageZoneElement.ElementId, cms.PageZoneElement.MasterPageId, cms.PageZoneElement.MasterPageZoneId, cms.PageZoneElement.MasterPageZoneElementId
	                    FROM cms.PageZoneElement INNER JOIN cms.Element ON
                        cms.PageZoneElement.TenantId = cms.Element.TenantId AND cms.PageZoneElement.ElementId = cms.Element.ElementId
                        WHERE cms.PageZoneElement.TenantId = @TenantId AND cms.PageZoneElement.PageId = @PageId AND cms.PageZoneElement.PageZoneId = @PageZoneId AND cms.PageZoneElement.PageZoneElementId = @PageZoneElementId",
                    new { TenantId = tenantId, PageId = pageId, PageZoneId = pageZoneId, PageZoneElementId = pageZoneElementId }
                );

                return pageZoneElement;
            }
        }
    }
}
