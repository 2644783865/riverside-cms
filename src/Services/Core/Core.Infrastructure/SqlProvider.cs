using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Core.Infrastructure
{
    public static class SqlProvider
    {
        public static string GetParentPageClause(long? parentPageId)
        {
            return parentPageId.HasValue ? "cms.[Page].ParentPageId = @ParentPageId" : "cms.[Page].ParentPageId IS NULL";
        }

        private static string GetInsertParentPageSql()
        {
            return @"
                -- Record the page that is passed to this stored procedure

	            INSERT INTO
		            @Folders (PageId)
	            SELECT
		            cms.[Page].PageId
	            FROM
		            cms.[Page]
	            INNER JOIN
		            cms.[MasterPage]
	            ON
		            cms.[Page].TenantId = cms.[MasterPage].TenantId AND
		            cms.[Page].MasterPageId = cms.[MasterPage].MasterPageId
	            WHERE
		            cms.[Page].TenantId = @TenantId AND
		            cms.[Page].PageId = @ParentPageId AND
		            cms.[MasterPage].PageType = 0 /* PageType.Folder */
            ";
        }

        public static string GetFoldersRecursiveSql(long? parentPageId)
        {
            string sql = $@"
                DECLARE @Folders TABLE(
                    PageId [bigint] NOT NULL PRIMARY KEY CLUSTERED
                )

                -- Record all child folders under page that is passed to this stored procedure

                ;WITH [Folders] AS
                (
                    SELECT
		                cms.[Page].PageId
                    FROM
                        cms.[Page]
	                INNER JOIN
		                cms.[MasterPage]
	                ON
		                cms.[Page].TenantId = cms.[MasterPage].TenantId AND
		                cms.[Page].MasterPageId = cms.[MasterPage].MasterPageId
                    WHERE
		                cms.[Page].TenantId = @TenantId AND
                        {GetParentPageClause(parentPageId)} AND
		                cms.[MasterPage].PageType = 0 /* PageType.Folder */
	                UNION ALL
	                SELECT
		                ChildPage.PageId
	                FROM
		                cms.[Page] ChildPage
	                INNER JOIN
		                cms.[MasterPage] ChildMasterPage
	                ON
		                ChildPage.TenantId = ChildMasterPage.TenantId AND
		                ChildPage.MasterPageId = ChildMasterPage.MasterPageId
	                INNER JOIN
		                [Folders]
	                ON
		                ChildPage.TenantId = @TenantId AND
		                ChildPage.ParentPageId = [Folders].PageId
	                WHERE
		                ChildMasterPage.PageType = 0 /* PageType.Folder */
                )

                INSERT INTO
	                @Folders (PageId)
                SELECT
	                [Folders].PageId
                FROM
	                [Folders]
                ";

            if (parentPageId.HasValue)
                sql += GetInsertParentPageSql();

            return sql;
        }

        public static string GetFoldersSql(long? parentPageId)
        {
            string sql = @"
                DECLARE @Folders TABLE(
                    PageId [bigint] NOT NULL PRIMARY KEY CLUSTERED
                )";

            if (parentPageId.HasValue)
                sql += GetInsertParentPageSql();

            return sql;
        }

        public static string GetTaggedPagesSql(long? parentPageId, string pagesTableName)
        {
            return $@"
                DECLARE @{pagesTableName} TABLE (
                    PageId bigint NOT NULL PRIMARY KEY CLUSTERED
                )

                DECLARE @TagCount int
                SELECT @TagCount = (SELECT COUNT(*) FROM @Tags)

                INSERT INTO
	                @{pagesTableName} (PageId)
                SELECT
	                cms.[Page].PageId
                FROM
	                cms.[Page]
                INNER JOIN
	                cms.TagPage
                ON
	                cms.[Page].TenantId = cms.TagPage.TenantId AND
	                cms.[Page].PageId = cms.TagPage.PageId
                INNER JOIN
	                @Tags Tags
                ON
	                cms.TagPage.TagId = Tags.TagId
                WHERE
	                cms.[Page].TenantId = @TenantId AND
                    {GetParentPageClause(parentPageId)}
                GROUP BY
	                cms.[Page].PageId
                HAVING
	                COUNT(Tags.TagId) = @TagCount
            ";
        }

        public static string GetTaggedPagesRecursiveSql(string pagesTableName)
        {
            return $@"
                DECLARE @{pagesTableName} TABLE (
                    PageId bigint NOT NULL PRIMARY KEY CLUSTERED
                )

                DECLARE @TagCount int
                SELECT @TagCount = (SELECT COUNT(*) FROM @Tags)

                INSERT INTO
	                @{pagesTableName} (PageId)
                SELECT
	                cms.[Page].PageId
                FROM
	                cms.[Page]
                INNER JOIN
	                @Folders [FoldersTable]
                ON
	                cms.[Page].TenantId = @TenantId AND
                    cms.[Page].ParentPageId = [FoldersTable].PageId
                INNER JOIN
	                cms.TagPage
                ON
	                cms.[Page].TenantId = cms.TagPage.TenantId AND
	                cms.[Page].PageId = cms.TagPage.PageId
                INNER JOIN
	                @Tags Tags
                ON
	                cms.TagPage.TagId = Tags.TagId
                GROUP BY
	                cms.[Page].PageId
                HAVING
	                COUNT(Tags.TagId) = @TagCount
            ";
        }

        public static string GetTagsSql()
        {
            return @"
                DECLARE @Tags TABLE (
                    TagId bigint NOT NULL PRIMARY KEY CLUSTERED
                )
                INSERT INTO
                    @Tags (TagId)
                SELECT
                    cms.Tag.TagId
                FROM
                    cms.Tag
                WHERE
                    cms.Tag.TenantId = @TenantId AND
                    cms.Tag.TagId IN @TagIds
            ";
        }
    }
}
