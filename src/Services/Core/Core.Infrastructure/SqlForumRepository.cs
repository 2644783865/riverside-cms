﻿using System;
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
    public class SqlForumRepository : IForumRepository
    {
        private readonly IOptions<SqlOptions> _options;

        private const string TaggedPagesTableName = "Pages";

        public SqlForumRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        private string GetListLatestThreadsSetupSql()
        {
            return @"
                DECLARE @Forums TABLE ([PageId] [bigint] NOT NULL PRIMARY KEY CLUSTERED, ElementId bigint NOT NULL)
            ";
        }

        private string GetListLatestThreadsPagesSql(long? parentPageId)
        {
            return $@"
                DECLARE @Pages TABLE ([PageId] [bigint] NOT NULL PRIMARY KEY CLUSTERED)
                INSERT INTO
	                @Pages (PageId)
                SELECT
	                cms.[Page].PageId
                FROM
	                cms.[Page]
                WHERE
	                cms.[Page].TenantId = @TenantId AND
                    {SqlProvider.GetParentPageClause(parentPageId)}
            ";
        }

        private string GetListLatestThreadsPagesRecursiveSql()
        {
            return @"
                DECLARE @Pages TABLE ([PageId] [bigint] NOT NULL PRIMARY KEY CLUSTERED)
                INSERT INTO
	                @Pages (PageId)
                SELECT
	                cms.[Page].PageId
                FROM
	                cms.[Page]
                INNER JOIN
	                @Folders [FoldersTable]
                ON
	                cms.[Page].TenantId = @TenantId AND
                    cms.[Page].ParentPageId = [FoldersTable].PageId
            ";
        }

        private string GetListLatestThreadsForumsSql()
        {
            return @"
                INSERT INTO
	                @Forums (PageId, ElementId)
                SELECT
	                cms.PageZoneElement.PageId,
	                cms.PageZoneElement.ElementId
                FROM
	                @Pages [PagesTable]
                INNER JOIN
	                cms.PageZoneElement
                ON
	                cms.PageZoneElement.PageId = [PagesTable].PageId
                INNER JOIN
	                element.Forum
                ON
	                cms.PageZoneElement.TenantId = element.Forum.TenantId AND
	                cms.PageZoneElement.ElementId = element.Forum.ElementId
                WHERE
	                cms.PageZoneElement.TenantId = @TenantId
            ";
        }

        private string GetListLatestThreadsThreadsSql()
        {
            return $@"
                DECLARE @RowNumberLowerBound int
                DECLARE @RowNumberUpperBound int
                SET @RowNumberLowerBound = @PageSize * @PageIndex
                SET @RowNumberUpperBound = @RowNumberLowerBound + @PageSize + 1;

                WITH ForumThreads AS
                (
	                SELECT TOP (@RowNumberUpperBound)
		                ROW_NUMBER() OVER (ORDER BY element.ForumThread.LastMessageCreated DESC) AS RowNumber,
		                [ForumsTable].PageId,
		                element.ForumThread.ElementId,
		                element.ForumThread.ThreadId
	                FROM
		                element.ForumThread
	                INNER JOIN
		                @Forums [ForumsTable]
	                ON
		                element.ForumThread.TenantId = @TenantId AND
		                element.ForumThread.ElementId = [ForumsTable].ElementId
                )

                SELECT
	                ForumThreads.PageId,
	                element.ForumThread.TenantId,
	                element.ForumThread.ElementId AS ForumId,
	                element.ForumThread.ThreadId,
	                element.ForumThread.UserId,
	                element.ForumThread.[Subject],
	                element.ForumThread.[Message],
	                element.ForumThread.Notify,
	                element.ForumThread.[Views],
	                element.ForumThread.Replies,
	                element.ForumThread.LastPostId,
	                element.ForumThread.LastMessageCreated AS LastPostCreated,
	                element.ForumThread.Created,
	                element.ForumThread.Updated,
	                LastPostUser.UserId AS LastPostUserId
                FROM
	                ForumThreads
                INNER JOIN
	                element.ForumThread
                ON
	                element.ForumThread.TenantId = @TenantId AND
	                element.ForumThread.ElementId = ForumThreads.ElementId AND
	                element.ForumThread.ThreadId = ForumThreads.ThreadId
                INNER JOIN
	                cms.[User]
                ON
	                element.ForumThread.TenantId = cms.[User].TenantId AND
	                element.ForumThread.UserId = cms.[User].UserId
                LEFT JOIN
	                element.ForumPost
                ON
	                element.ForumThread.TenantId = element.ForumPost.TenantId AND
	                element.ForumThread.ElementId = element.ForumPost.ElementId AND
	                element.ForumThread.ThreadId = element.ForumPost.ThreadId  AND
	                element.ForumThread.LastPostId = element.ForumPost.PostId
                LEFT JOIN
	                cms.[User] LastPostUser 
                ON
	                element.ForumPost.TenantId = LastPostUser.TenantId AND
	                element.ForumPost.UserId = LastPostUser.UserId
                LEFT JOIN
	                cms.Upload
                ON
	                cms.[User].ImageTenantId = cms.Upload.TenantId AND
	                cms.[User].ThumbnailImageUploadId = cms.Upload.UploadId
                LEFT JOIN
	                cms.[Image]
                ON
	                cms.Upload.TenantId = cms.[Image].TenantId AND
	                cms.Upload.UploadId = cms.[Image].UploadId
                LEFT JOIN
	                cms.Upload LastPostUpload
                ON
	                LastPostUser.ImageTenantId = LastPostUpload.TenantId AND
	                LastPostUser.ThumbnailImageUploadId = LastPostUpload.UploadId
                LEFT JOIN
	                cms.[Image] LastPostImage
                ON
	                LastPostUpload.TenantId = LastPostImage.TenantId AND
	                LastPostUpload.UploadId = LastPostImage.UploadId
                WHERE
	                ForumThreads.RowNumber > @RowNumberLowerBound AND ForumThreads.RowNumber < @RowNumberUpperBound
                ORDER BY
	                ForumThreads.RowNumber ASC
            ";
        }

        private string GetListLatestThreadsSql(long? parentPageId)
        {
            return $@"
                {GetListLatestThreadsSetupSql()}
                {GetListLatestThreadsPagesSql(parentPageId)}
                {GetListLatestThreadsForumsSql()}
                {GetListLatestThreadsThreadsSql()}
            ";
        }

        private string GetListLatestThreadsRecursiveSql(long? parentPageId)
        {
            return $@"
                {GetListLatestThreadsSetupSql()}
                {SqlProvider.GetFoldersRecursiveSql(parentPageId)}
                {GetListLatestThreadsPagesRecursiveSql()}
                {GetListLatestThreadsForumsSql()}
                {GetListLatestThreadsThreadsSql()}
            ";
        }

        private string GetListTaggedLatestThreadsSql(long? parentPageId)
        {
            return $@"
                {GetListLatestThreadsSetupSql()}
                {SqlProvider.GetTagsSql()}
                {SqlProvider.GetTaggedPagesSql(parentPageId, TaggedPagesTableName)}
                {GetListLatestThreadsForumsSql()}
                {GetListLatestThreadsThreadsSql()}
            ";
        }

        private string GetListTaggedLatestThreadsRecursiveSql(long? parentPageId)
        {
            return $@"
                {GetListLatestThreadsSetupSql()}
                {SqlProvider.GetFoldersRecursiveSql(parentPageId)}
                {SqlProvider.GetTagsSql()}
                {SqlProvider.GetTaggedPagesRecursiveSql(TaggedPagesTableName)}
                {GetListLatestThreadsForumsSql()}
                {GetListLatestThreadsThreadsSql()}
            ";
        }

        private string GetListLatestThreadsSql(long? parentPageId, bool recursive, IEnumerable<long> tagIds)
        {
            string sql = null;
            bool filterByTags = tagIds != null && tagIds.Count() > 0;
            if (filterByTags)
            {
                if (recursive)
                    sql = GetListTaggedLatestThreadsRecursiveSql(parentPageId);
                else
                    sql = GetListTaggedLatestThreadsSql(parentPageId);
            }
            else
            {
                if (recursive)
                    sql = GetListLatestThreadsRecursiveSql(parentPageId);
                else
                    sql = GetListLatestThreadsSql(parentPageId);
            }
            return sql;
        }

        public async Task<IEnumerable<ForumThread>> ListLatestThreadsAsync(long tenantId, long? parentPageId, bool recursive, IEnumerable<long> tagIds, int pageSize)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<ForumThread>(
                    GetListLatestThreadsSql(parentPageId, recursive, tagIds),
                    new
                    {
                        TenantId = tenantId,
                        ParentPageId = parentPageId,
                        PageIndex = 0,
                        PageSize = pageSize,
                        TagIds = tagIds
                    }
                );
            }
        }
    }
}
