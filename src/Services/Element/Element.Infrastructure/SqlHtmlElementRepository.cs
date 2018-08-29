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
    public class SqlHtmlElementRepository : IElementRepository<HtmlElementSettings>
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlHtmlElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<HtmlElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
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
                        element.Html.Html
                    FROM
                        cms.Element
                    INNER JOIN
                        element.Html
                    ON
                        cms.Element.TenantId = element.Html.TenantId AND
                        cms.Element.ElementId = element.Html.ElementId
                    WHERE
                        cms.Element.TenantId = @TenantId AND
                        cms.Element.ElementId = @ElementId
                    SELECT
                        element.HtmlUpload.HtmlUploadId AS HtmlBlobId,
	                    element.HtmlUpload.ImageUploadId AS ImageBlobId,
	                    element.HtmlUpload.PreviewImageUploadId AS PreviewImageBlobId,
	                    element.HtmlUpload.ThumbnailImageUploadId AS ThumbnailImageBlobId
                    FROM
                        element.HtmlUpload
                    WHERE
	                    element.HtmlUpload.TenantId = @TenantId AND
                        element.HtmlUpload.ElementId = @ElementId",
                    new
                    {
                        TenantId = tenantId,
                        ElementId = elementId
                    }
                ))
                {
                    HtmlElementSettings settings = await gr.ReadFirstOrDefaultAsync<HtmlElementSettings>();
                    if (settings != null)
                        settings.Blobs = await gr.ReadAsync<HtmlBlob>();
                    return settings;
                }
            }
        }
    }
}
