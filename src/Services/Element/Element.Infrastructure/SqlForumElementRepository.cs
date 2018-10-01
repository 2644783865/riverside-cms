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
    public class SqlForumElementRepository : IElementRepository<ForumElementSettings>
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlForumElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<ForumElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();

                return await connection.QueryFirstOrDefaultAsync<ForumElementSettings>(@"
                    SELECT
                        cms.Element.TenantId,
                        cms.Element.ElementId,
                        cms.Element.ElementTypeId,
                        cms.Element.Name,
                        element.Forum.OwnerTenantId,
                        element.Forum.OwnerUserId,
                        element.Forum.OwnerOnlyThreads
                    FROM
                        cms.Element
                    INNER JOIN
                        element.Forum
                    ON
                        cms.Element.TenantId = element.Forum.TenantId AND
                        cms.Element.ElementId = element.Forum.ElementId
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
