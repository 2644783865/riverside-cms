using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Infrastructure
{
    public class SqlPageRepository : IPageRepository
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlPageRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<Page> ReadPageAsync(long tenantId, long pageId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                Page page = await connection.QueryFirstOrDefaultAsync<Page>(
                    @"SELECT TenantId, PageId, ParentPageId, Name, Description, Created, Updated, Occurred, MasterPageId,
                        ImageUploadId, PreviewImageUploadId, ThumbnailImageUploadId
                        FROM cms.Page WHERE TenantId = @TenantId AND PageId = @PageId",
                    new { TenantId = tenantId, PageId = pageId }
                );
                return page;
            }
        }

        public async Task<IEnumerable<Page>> ListPagesInHierarchyAsync(long tenantId, long pageId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                IEnumerable<Page> pages = await connection.QueryAsync<Page>(
                    @";WITH [Pages] AS
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
	                        cms.[Page].ThumbnailImageUploadId,
	                        cms.[Page].PreviewImageUploadId,
	                        cms.[Page].ImageUploadId
                        FROM
	                        cms.[Page]
                        INNER JOIN
	                        [Pages]
                        ON
	                        cms.[Page].TenantId = @TenantId AND
	                        cms.[Page].PageId = [Pages].PageId
                        ORDER BY
	                        [Pages].[Level] ASC",
                    new { TenantId = tenantId, PageId = pageId }
                );
                return pages;
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
