using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Element.Domain;

namespace Riverside.Cms.Services.Element.Infrastructure
{
    public class SqlLatestThreadsElementRepository : IElementRepository<LatestThreadsElementSettings>
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlLatestThreadsElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<LatestThreadsElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<LatestThreadsElementSettings>(@"
                    SELECT
                        cms.Element.TenantId,
                        cms.Element.ElementId,
                        cms.Element.ElementTypeId,
                        cms.Element.Name,
	                    element.LatestThread.PageTenantId,
	                    element.LatestThread.PageId,
	                    element.LatestThread.DisplayName,
	                    element.LatestThread.[Recursive],
	                    element.LatestThread.NoThreadsMessage,
	                    element.LatestThread.Preamble,
	                    element.LatestThread.PageSize
                    FROM
                        cms.Element
                    INNER JOIN
	                    element.LatestThread
                    ON
                        cms.Element.TenantId = element.LatestThread.TenantId AND
                        cms.Element.ElementId = element.LatestThread.ElementId
                    WHERE
	                    element.LatestThread.TenantId = @TenantId AND
	                    element.LatestThread.ElementId = @ElementId",
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
