using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Element.Domain;
using Riverside.Cms.Services.Mortgage.Domain;

namespace Riverside.Cms.Services.Mortgage.Infrastructure
{
    public class SqlCalculatorElementRepository<TElementSettings> : IElementRepository<TElementSettings> where TElementSettings : IElementSettings
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlCalculatorElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<TElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<TElementSettings>(@"
                    SELECT
                        cms.Element.TenantId,
                        cms.Element.ElementId,
                        cms.Element.ElementTypeId,
                        cms.Element.Name
                    FROM
                        cms.Element
                    WHERE
                        cms.Element.TenantId = @TenantId AND
                        cms.Element.ElementId = @ElementId",
                    new
                    {
                        TenantId = tenantId,
                        ElementId = elementId
                    }
                );
            }
        }
    }
}
