using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Element.Domain;

namespace Riverside.Cms.Services.Element.Infrastructure
{
    public class SqlTableElementRepository : IElementRepository<TableElementSettings>
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlTableElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<TableElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<TableElementSettings>(@"
                    SELECT
                        cms.Element.TenantId,
                        cms.Element.ElementId,
                        cms.Element.ElementTypeId,
                        cms.Element.Name,
                        element.[Table].DisplayName,
                        element.[Table].Preamble,
                        element.[Table].ShowHeaders,
                        element.[Table].Rows
                    FROM
                        cms.Element
                    INNER JOIN
                        element.[Table]
                    ON
                        cms.Element.TenantId = element.[Table].TenantId AND
                        cms.Element.ElementId = element.[Table].ElementId
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
