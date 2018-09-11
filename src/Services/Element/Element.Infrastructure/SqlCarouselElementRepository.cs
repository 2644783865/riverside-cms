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
    public class SqlCarouselElementRepository : IElementRepository<CarouselElementSettings>
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlCarouselElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<CarouselElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            using (SqlConnection connection = new SqlConnection(_options.Value.SqlConnectionString))
            {
                connection.Open();
                using (GridReader gr = await connection.QueryMultipleAsync(@"
                    SELECT
                        cms.Element.TenantId,
                        cms.Element.ElementId,
                        cms.Element.ElementTypeId,
                        cms.Element.Name
                    FROM
                        cms.Element
                    WHERE
                        cms.Element.TenantId = @TenantId AND
                        cms.Element.ElementId = @ElementId AND
                        cms.Element.ElementTypeId = @ElementTypeId
                    SELECT
                        element.CarouselSlide.CarouselSlideId AS BlobSetId,
	                    element.CarouselSlide.ImageUploadId AS ImageBlobId,
	                    element.CarouselSlide.PreviewImageUploadId AS PreviewImageBlobId,
	                    element.CarouselSlide.ThumbnailImageUploadId AS ThumbnailImageBlobId,
                        element.CarouselSlide.Name,
                        element.CarouselSlide.Description,
                        element.CarouselSlide.PageId AS ButtonPageId,
                        element.CarouselSlide.PageText AS ButtonText
                    FROM
                        element.CarouselSlide
                    WHERE
	                    element.CarouselSlide.TenantId = @TenantId AND
                        element.CarouselSlide.ElementId = @ElementId
                    ORDER BY
                        element.CarouselSlide.SortOrder",
                    new
                    {
                        TenantId = tenantId,
                        ElementId = elementId,
                        ElementTypeId = new Guid("aacb11a0-5532-47cb-aab9-939cee3d5175")
                    }
                ))
                {
                    CarouselElementSettings settings = await gr.ReadFirstOrDefaultAsync<CarouselElementSettings>();
                    if (settings != null)
                        settings.Slides = await gr.ReadAsync<CarouselSlide>();
                    return settings;
                }
            }
        }
    }
}
