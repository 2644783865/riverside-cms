using System.Data.SqlClient;
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

        public async Task<NavigationBarElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();

                using (GridReader gr = await connection.QueryMultipleAsync(
                    @"SELECT cms.Element.TenantId, cms.Element.ElementId, cms.Element.ElementTypeId, cms.Element.Name,
                        element.NavBar.Name AS NavBarName, element.NavBar.ShowLoggedOnUserOptions, element.NavBar.ShowLoggedOffUserOptions
                        FROM cms.Element INNER JOIN element.NavBar ON cms.Element.TenantId = element.NavBar.TenantId AND cms.Element.ElementId = element.NavBar.ElementId
                        WHERE cms.Element.TenantId = @TenantId AND cms.Element.ElementId = @ElementId
                      SELECT element.NavBarTab.TenantId, element.NavBarTab.ElementId, element.NavBarTab.NavBarTabId AS NavigationBarTabId, element.NavBarTab.Name, element.NavBarTab.SortOrder, element.NavBarTab.PageId
                        FROM element.NavBarTab
                        WHERE element.NavBarTab.TenantId = @TenantId AND element.NavBarTab.ElementId = @elementId
                        ORDER BY element.NavBarTab.SortOrder",
                    new { TenantId = tenantId, ElementId = elementId }
                    )
                )
                {
                    NavigationBarElementSettings elementSettings = await gr.ReadFirstOrDefaultAsync<NavigationBarElementSettings>();
                    elementSettings.Tabs = await gr.ReadAsync<NavigationBarTab>();
                    return elementSettings;
                }
            }
        }
    }
}
