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
    public class SqlDomainRepository : IDomainRepository
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlDomainRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<WebDomain> ReadDomainByUrlAsync(string url)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<WebDomain>(@"
                    SELECT
                        TenantId,
                        DomainId,
                        Url,
                        RedirectUrl
                    FROM
                        cms.Domain
                    WHERE
                        Url = @Url",
                    new
                    {
                        Url = url
                    }
                );
            }
        }
    }
}
