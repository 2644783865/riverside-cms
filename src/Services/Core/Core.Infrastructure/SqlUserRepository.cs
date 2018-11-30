using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Core.Domain;
using static Dapper.SqlMapper;

namespace Riverside.Cms.Services.Core.Infrastructure
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlUserRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<User> ReadUserAsync(long tenantId, long userId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<User>(@"
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
                        UserId = @UserId",
                    new
                    {
                        TenantId = tenantId,
                        UserId = userId
                    }
                );
            }
        }

        public async Task<UserIdentity> ReadUserIdentityAsync(long tenantId, string email)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                using (GridReader gr = await connection.QueryMultipleAsync(@"
                    SELECT
                        cms.[User].TenantId,
                        cms.[User].UserId,
                        cms.[User].Alias,
                        cms.[User].Email
                    FROM
                        cms.[User]
                    WHERE
                        TenantId = @TenantId AND
                        Email = @Email

                    SELECT
	                    cms.[Role].Name
                    FROM
	                    cms.[Role]
                    INNER JOIN
	                    cms.[UserRole]
                    ON
	                    cms.[Role].RoleId = cms.[UserRole].RoleId
                    INNER JOIN
                        cms.[User]
                    ON
                        cms.[UserRole].TenantId = cms.[User].TenantId AND
                        cms.[UserRole].UserId = cms.[User].UserId
                    WHERE
	                    cms.[User].TenantId = @TenantId AND
	                    cms.[User].Email = @Email",
                    new
                    {
                        TenantId = tenantId,
                        Email = email
                    }))
                {
                    UserIdentity identity = await gr.ReadFirstOrDefaultAsync<UserIdentity>();
                    if (identity != null)
                        identity.Roles = await gr.ReadAsync<string>();
                    return identity;
                }
            }
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