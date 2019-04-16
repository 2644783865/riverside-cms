using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Infrastructure
{
    public class SqlWebRepository : IWebRepository
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlWebRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<Web> ReadWebAsync(long tenantId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<Web>(@"
                    SELECT
                        TenantId,
                        Name,
                        GoogleSiteVerification,
                        HeadScript
                    FROM
                        cms.Web
                    WHERE
                        TenantId = @TenantId",
                    new
                    {
                        TenantId = tenantId
                    }
                );
            }
        }

        public async Task UpdateWebAsync(long tenantId, Web web)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                await connection.ExecuteAsync(@"
                    UPDATE
                        cms.[Web]
                    SET
                        cms.[Web].Name = @Name,
                        cms.[Web].GoogleSiteVerification = @GoogleSiteVerification,
                        cms.[Web].HeadScript = @HeadScript
                    WHERE
                        cms.[Web].TenantId = @TenantId",
                    new
                    {
                        web.Name,
                        web.GoogleSiteVerification,
                        web.HeadScript,
                        TenantId = tenantId
                    }
                );
            }
        }
    }
}
