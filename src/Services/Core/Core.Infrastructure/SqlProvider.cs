using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Core.Infrastructure
{
    public static class SqlProvider
    {
        public static string GetFoldersRecursiveSql()
        {
            return @"
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
                        cms.[Page].ParentPageId = @ParentPageId AND
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

        public static string GetFoldersSql()
        {
            return @"
                DECLARE @Folders TABLE(
                    PageId [bigint] NOT NULL PRIMARY KEY CLUSTERED
                )

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
    }
}
