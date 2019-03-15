using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Auth.Domain;

namespace Riverside.Cms.Services.Auth.Infrastructure
{
    public class SqlAuthenticationRepository : IAuthenticationRepository
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlAuthenticationRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<AuthenticationState> ReadAuthenticationStateAsync(long tenantId, string email)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<AuthenticationState>(@"
                SELECT
                    cms.[User].TenantId,
                    cms.[User].Email,
                    cms.[User].Confirmed,
                    cms.[User].Enabled,
                    cms.[User].LockedOut,
                    cms.[User].PasswordSaltedHash,
                    cms.[User].PasswordSalt,
                    cms.[User].LastPasswordFailure,
                    cms.[User].PasswordFailures
                FROM
                    cms.[User]
                WHERE
                    TenantId = @TenantId AND
                    Email = @Email",
                    new
                    {
                        TenantId = tenantId,
                        Email = email
                    }
                );
            }
        }

        public async Task UpdateAuthenticationStateAsync(long tenantId, string email, AuthenticationState state)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                await connection.ExecuteAsync(@";
                    UPDATE
                        cms.[User]
                    SET 
                        Confirmed = @Confirmed,
                        Enabled = @Enabled,
                        LockedOut = @LockedOut,
                        PasswordSaltedHash = @PasswordSaltedHash,
                        PasswordSalt = @PasswordSalt,
                        LastPasswordFailure = @LastPasswordFailure,
                        PasswordFailures = @PasswordFailures
                    WHERE
                        TenantId = @TenantId AND
                        Email = @Email",
                    new
                    {
                        TenantId = tenantId,
                        Email = email,
                        state.Confirmed,
                        state.Enabled,
                        state.LockedOut,
                        state.PasswordSaltedHash,
                        state.PasswordSalt,
                        state.LastPasswordFailure,
                        state.PasswordFailures
                    }
                );
            }
        }
    }
}
