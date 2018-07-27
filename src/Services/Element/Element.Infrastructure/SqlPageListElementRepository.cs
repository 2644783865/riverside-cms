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
    public class SqlPageListElementRepository : IElementRepository<PageListElementSettings>
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlPageListElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<PageListElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();

                return await connection.QueryFirstOrDefaultAsync<PageListElementSettings>(@"
                    SELECT
                        cms.Element.TenantId,
                        cms.Element.ElementId,
                        cms.Element.ElementTypeId,
                        cms.Element.Name,
                        element.PageList.PageId,
                        element.PageList.DisplayName,
                        element.PageList.SortBy,
                        element.PageList.SortAsc,
                        element.PageList.ShowDescription,
                        element.PageList.ShowImage,
                        element.PageList.ShowBackgroundImage,
                        element.PageList.ShowCreated,
                        element.PageList.ShowUpdated,
                        element.PageList.ShowOccurred,
                        element.PageList.ShowComments,
                        element.PageList.ShowTags,
                        element.PageList.ShowPager,
                        element.PageList.MoreMessage,
                        element.PageList.Recursive,
                        element.PageList.PageType,
                        element.PageList.PageSize,
                        element.PageList.NoPagesMessage,
                        element.PageList.Preamble
                    FROM
                        cms.Element
                    INNER JOIN
                        element.PageList
                    ON
                        cms.Element.TenantId = element.PageList.TenantId AND
                        cms.Element.ElementId = element.PageList.ElementId
                    WHERE
                        cms.Element.TenantId = @TenantId AND
                        cms.Element.ElementId = @ElementId",
                    new { TenantId = tenantId, ElementId = elementId }
                );
            }
        }
    }
}
