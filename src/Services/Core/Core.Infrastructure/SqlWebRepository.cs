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
                        Name
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
    }
}
