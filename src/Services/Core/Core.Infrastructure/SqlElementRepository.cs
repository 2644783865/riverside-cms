using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Infrastructure
{
    public class SqlElementRepository : IElementRepository
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<ElementDefinition> ReadElementDefinitionAsync(long tenantId, Guid elementTypeId, long elementId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<ElementDefinition>(@"
                    SELECT
                        TenantId,
                        ElementId,
                        ElementTypeId,
                        Name
                    FROM
                        cms.Element
                    WHERE
                        TenantId = @TenantId AND
                        ElementId = @ElementId",
                    new
                    {
                        TenantId = tenantId,
                        ElementId = elementId
                    }
                );
            }
        }

        public async Task<IEnumerable<ElementDefinition>> ListElementDefinitionsAsync(long tenantId, Guid elementTypeId, IEnumerable<long> elementIds)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<ElementDefinition>(@"
                    SELECT
                        TenantId,
                        ElementId,
                        ElementTypeId,
                        Name
                    FROM
                        cms.Element
                    WHERE
                        TenantId = @TenantId AND
                        ElementId IN @ElementIds",
                    new
                    {
                        TenantId = tenantId,
                        ElementIds = elementIds
                    }
                );
            }
        }

        public async Task<IEnumerable<ElementType>> ListElementTypesAsync()
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryAsync<ElementType>(@"
                    SELECT
                        ElementTypeId,
                        Name
                    FROM
                        cms.ElementType
                    ORDER BY
    	                ElementTypeId"
                );
            }
        }
    }
}
