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
    public class SqlTagCloudElementRepository : IElementRepository<TagCloudElementSettings>
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlTagCloudElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<TagCloudElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<TagCloudElementSettings>(@"
                    SELECT
                        cms.Element.TenantId,
                        cms.Element.ElementId,
                        cms.Element.ElementTypeId,
                        cms.Element.Name,
                        element.TagCloud.PageId,
                        element.TagCloud.DisplayName,
                        element.TagCloud.Recursive,
                        element.TagCloud.NoTagsMessage
                    FROM
                        cms.Element
                    INNER JOIN
                        element.TagCloud
                    ON 
                        cms.Element.TenantId = element.TagCloud.TenantId AND
                        cms.Element.ElementId = element.TagCloud.ElementId
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
