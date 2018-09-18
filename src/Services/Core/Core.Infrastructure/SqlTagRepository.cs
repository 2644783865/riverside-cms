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
    public class SqlTagRepository : ITagRepository
    {
        private readonly IOptions<SqlOptions> _options;

        private const string TaggedPagesTableName = "Pages";

        public SqlTagRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<Tag> ReadTagAsync(long tenantId, long tagId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<Tag>(@"
                    SELECT
                        cms.Tag.TagId,
                        cms.Tag.Name
                    FROM
                        cms.Tag
                    WHERE
                        TenantId = @TenantId AND
                        TagId = @TagId",
                    new
                    {
                        TenantId = tenantId,
                        TagId = tagId
                    }
                );
            }
        }

        public async Task<IEnumerable<Tag>> ListTagsAsync(long tenantId, IEnumerable<long> tagIds)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<Tag>(@"
                    SELECT
                        cms.Tag.TagId,
                        cms.Tag.Name
                    FROM
                        cms.Tag
                    WHERE
                        TenantId = @TenantId AND
                        TagId IN @TagIds
                    ORDER BY
    	                Name",
                    new
                    {
                        TenantId = tenantId,
                        TagIds = tagIds
                    }
                );
            }
        }

        public async Task<IEnumerable<Tag>> ListTagsAsync(long tenantId, IEnumerable<string> tagNames)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<Tag>(@"
                    SELECT
                        cms.Tag.TagId,
                        cms.Tag.Name
                    FROM
                        cms.Tag
                    WHERE
                        TenantId = @TenantId AND
                        Name IN @TagNames
                    ORDER BY
    	                Name
                    ",
                    new
                    {
                        TenantId = tenantId,
                        TagNames = tagNames
                    }
                );
            }
        }

        private string GetListTagsSelectSql()
        {
            return @"
                SELECT
	                COUNT(cms.Tag.TagId) AS Count,
	                cms.Tag.TagId,
	                cms.Tag.Name
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
                INNER JOIN
	                cms.TagPage 
                ON
	                cms.[Page].TenantId = cms.TagPage.TenantId AND
	                cms.[Page].PageId = cms.TagPage.PageId
                INNER JOIN
	                cms.Tag
                ON
	                cms.TagPage.TenantId = cms.Tag.TenantId AND
	                cms.TagPage.TagId = cms.Tag.TagId
                WHERE
	                cms.MasterPage.PageType	= 1 /* PageType.Document */
                GROUP BY
	                cms.Tag.TagId,
	                cms.Tag.Name
                ORDER BY
	                cms.Tag.Name
            ";
        }

        private string GetListTagsRecursiveSql(long? parentPageId)
        {
            return $@"
                {SqlProvider.GetFoldersRecursiveSql(parentPageId)}
                {GetListTagsSelectSql()}
            ";
        }

        private string GetListTagsSql(long? parentPageId)
        {
            return $@"
                {SqlProvider.GetFoldersSql(parentPageId)}
                {GetListTagsSelectSql()}
            ";
        }

        private string GetListTagsSql(long? parentPageId, bool recursive)
        {
            if (recursive)
                return GetListTagsRecursiveSql(parentPageId);
            else
                return GetListTagsSql(parentPageId);
        }

        public async Task<IEnumerable<TagCount>> ListTagCountsAsync(long tenantId, long? parentPageId, bool recursive)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<TagCount>(
                    GetListTagsSql(parentPageId, recursive),
                    new
                    {
                        TenantId = tenantId,
                        ParentPageId = parentPageId
                    }
                );
            }
        }

        private string GetListRelatedTagsSelectSql()
        {
            return @"
                SELECT
	                COUNT(cms.Tag.TagId) AS Count,
	                cms.Tag.TagId,
	                cms.Tag.Name
                FROM
	                @Pages [PagesTable]
                INNER JOIN
	                cms.[Page]
                ON
	                cms.[Page].PageId = [PagesTable].PageId
                INNER JOIN
	                cms.[MasterPage]
                ON
	                cms.[Page].TenantId = cms.MasterPage.TenantId AND
	                cms.[Page].MasterPageId = cms.MasterPage.MasterPageId
                INNER JOIN
	                cms.TagPage 
                ON
	                cms.[Page].TenantId = cms.TagPage.TenantId AND
	                cms.[Page].PageId = cms.TagPage.PageId
                INNER JOIN
	                cms.[Tag]
                ON
	                cms.TagPage.TenantId = cms.Tag.TenantId AND
	                cms.TagPage.TagId = cms.Tag.TagId
                LEFT JOIN
	                @Tags Tags
                ON
	                cms.Tag.TagId = Tags.TagId
                WHERE
	                cms.[Page].TenantId = @TenantId AND
	                cms.MasterPage.PageType = 1 /* PageType.Document */ AND
	                Tags.TagId IS NULL
                GROUP BY
	                cms.Tag.TagId,
	                cms.Tag.Name
                ORDER BY
	                cms.Tag.Name
            ";
        }

        private string GetListRelatedTagsSql(long? parentPageId)
        {
            return $@"
                {SqlProvider.GetTagsSql()}
                {SqlProvider.GetFoldersSql(parentPageId)}
                {SqlProvider.GetTaggedPagesRecursiveSql(TaggedPagesTableName)}
                {GetListRelatedTagsSelectSql()}
            ";
        }

        private string GetListRelatedTagsRecursiveSql(long? parentPageId)
        {
            return $@"
                {SqlProvider.GetTagsSql()}
                {SqlProvider.GetFoldersRecursiveSql(parentPageId)}
                {SqlProvider.GetTaggedPagesRecursiveSql(TaggedPagesTableName)}
                {GetListRelatedTagsSelectSql()}
            ";
        }

        private string GetListRelatedTagsSql(long? parentPageId, bool recursive)
        {
            if (recursive)
                return GetListRelatedTagsRecursiveSql(parentPageId);
            else
                return GetListRelatedTagsSql(parentPageId);
        }

        public async Task<IEnumerable<TagCount>> ListRelatedTagCountsAsync(long tenantId, IEnumerable<long> tagIds, long? parentPageId, bool recursive)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<TagCount>(
                    GetListRelatedTagsSql(parentPageId, recursive),
                    new
                    {
                        TenantId = tenantId,
                        ParentPageId = parentPageId,
                        TagIds = tagIds
                    }
                );
            }
        }
    }
}
