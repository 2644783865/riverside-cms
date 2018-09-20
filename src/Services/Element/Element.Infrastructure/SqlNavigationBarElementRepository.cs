using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Element.Domain;
using static Dapper.SqlMapper;

namespace Riverside.Cms.Services.Element.Infrastructure
{
    public class SqlNavigationBarElementRepository : IElementRepository<NavigationBarElementSettings>
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlNavigationBarElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        private NavigationBarTab GetNavigationBarTabFromDto(NavigationBarTabDto dto)
        {
            return new NavigationBarTab
            {
                Name = dto.Name,
                PageId = dto.PageId,
                TabId = dto.TabId,
                Tabs = Enumerable.Empty<NavigationBarTab>()
            };
        }

        private IEnumerable<NavigationBarTab> GetTabs(IEnumerable<NavigationBarTab> parentTabs, IEnumerable<NavigationBarTabDto> childTabs)
        {
            foreach (NavigationBarTab parentTab in parentTabs)
            {
                parentTab.Tabs = childTabs
                    .Where(t => t.ParentTabId == parentTab.TabId)
                    .Select(t => GetNavigationBarTabFromDto(t));
                yield return parentTab;
            }
        }

        public async Task<NavigationBarElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();

                using (GridReader gr = await connection.QueryMultipleAsync(@"
                    SELECT
                        cms.Element.TenantId,
                        cms.Element.ElementId,
                        cms.Element.ElementTypeId,
                        cms.Element.Name,
                        ISNULL(element.NavBar.Name, '') AS Brand,
                        element.NavBar.ShowLoggedOnUserOptions,
                        element.NavBar.ShowLoggedOffUserOptions
                    FROM
                        cms.Element
                    INNER JOIN
                        element.NavBar
                    ON
                        cms.Element.TenantId = element.NavBar.TenantId AND
                        cms.Element.ElementId = element.NavBar.ElementId
                    WHERE
                        cms.Element.TenantId = @TenantId AND cms.Element.ElementId = @ElementId
                    SELECT
                        element.NavBarTab.NavBarTabId AS TabId,
                        element.NavBarTab.Name,
                        element.NavBarTab.PageId
                    FROM
                        element.NavBarTab
                    WHERE
                        element.NavBarTab.TenantId = @TenantId AND
                        element.NavBarTab.ElementId = @elementId AND
                        element.NavBarTab.ParentNavBarTabId IS NULL
                    ORDER BY
                        element.NavBarTab.SortOrder
                    SELECT
                        element.NavBarTab.NavBarTabId AS TabId,
                        element.NavBarTab.Name,
                        element.NavBarTab.PageId,
                        element.NavBarTab.ParentNavBarTabId AS ParentTabId
                    FROM
                        element.NavBarTab
                    WHERE
                        element.NavBarTab.TenantId = @TenantId AND
                        element.NavBarTab.ElementId = @elementId AND
                        element.NavBarTab.ParentNavBarTabId IS NOT NULL
                    ORDER BY
                        element.NavBarTab.ParentNavBarTabId,
                        element.NavBarTab.SortOrder",
                    new
                    {
                        TenantId = tenantId,
                        ElementId = elementId
                    }
                ))
                {
                    NavigationBarElementSettings settings = await gr.ReadFirstOrDefaultAsync<NavigationBarElementSettings>();
                    if (settings != null)
                    {
                        IEnumerable<NavigationBarTab> parentTabs = await gr.ReadAsync<NavigationBarTab>();
                        IEnumerable<NavigationBarTabDto> childTabs = await gr.ReadAsync<NavigationBarTabDto>();
                        settings.Tabs = GetTabs(parentTabs, childTabs);
                    }
                    return settings;
                }
            }
        }
    }
}
