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
    public class SqlSocialBarElementRepository : IElementRepository<SocialBarElementSettings>
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlSocialBarElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<SocialBarElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<SocialBarElementSettings>(@"
                    SELECT
	                    cms.Element.TenantId,
                        cms.Element.ElementId,
                        cms.Element.ElementTypeId,
                        cms.Element.Name,
	                    element.Contact.DisplayName,
	                    element.Contact.Preamble,
	                    element.Contact.[Address],
	                    element.Contact.Email,
	                    element.Contact.FacebookUsername,
	                    element.Contact.InstagramUsername,
	                    element.Contact.LinkedInCompanyUsername,
	                    element.Contact.LinkedInPersonalUsername,
	                    element.Contact.TelephoneNumber1,
	                    element.Contact.TelephoneNumber2,
	                    element.Contact.TwitterUsername,
	                    element.Contact.YouTubeChannelId
                    FROM
	                    cms.Element
                    INNER JOIN
                        element.Contact
                    ON
                        cms.Element.TenantId = element.Contact.TenantId AND
                        cms.Element.ElementId = element.Contact.ElementId
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
