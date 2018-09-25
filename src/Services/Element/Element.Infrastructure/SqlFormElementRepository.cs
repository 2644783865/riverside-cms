using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Element.Domain;
using static Dapper.SqlMapper;

namespace Riverside.Cms.Services.Element.Infrastructure
{
    public class SqlFormElementRepository : IElementRepository<FormElementSettings>
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlFormElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<FormElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
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
                        element.Form.RecipientEmail,
                        element.Form.SubmitButtonLabel,
                        element.Form.SubmittedMessage
                    FROM
                        cms.Element
                    INNER JOIN
                        element.Form
                    ON
                        cms.Element.TenantId = element.Form.TenantId AND
                        cms.Element.ElementId = element.Form.ElementId
                    WHERE
                        cms.Element.TenantId = @TenantId AND
                        cms.Element.ElementId = @ElementId
                    SELECT
                        element.FormField.FormFieldId,
	                    element.FormField.FormFieldType AS FieldType,
	                    element.FormField.Label,
	                    element.FormField.Required
                    FROM
                        element.FormField
                    WHERE
	                    element.FormField.TenantId = @TenantId AND
                        element.FormField.ElementId = @ElementId
                    ORDER BY
                        element.FormField.SortOrder",
                    new
                    {
                        TenantId = tenantId,
                        ElementId = elementId
                    }
                ))
                {
                    FormElementSettings settings = await gr.ReadFirstOrDefaultAsync<FormElementSettings>();
                    if (settings != null)
                        settings.Fields = await gr.ReadAsync<FormField>();
                    return settings;
                }
            }
        }
    }
}
