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
    public class SqlUserRepository : IUserRepository
    {
        private readonly IOptions<SqlOptions> _options;

        private const string TaggedPagesTableName = "TaggedPages";

        public SqlUserRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<IEnumerable<User>> ListUsersAsync(long tenantId, IEnumerable<long> userIds)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<User>(@"
                    SELECT
                        cms.[User].TenantId,
                        cms.[User].UserId,
                        cms.[User].Alias,
                        cms.[User].ImageUploadId AS ImageBlobId,
                        cms.[User].PreviewImageUploadId AS PreviewImageBlobId,
                        cms.[User].ThumbnailImageUploadId AS ThumbnailImageBlobId
                    FROM
                        cms.[User]
                    WHERE
                        TenantId = @TenantId AND
                        UserId IN @UserIds
                    ORDER BY
    	                Alias",
                    new
                    {
                        TenantId = tenantId,
                        UserIds = userIds
                    }
                );
            }
        }
    }
}