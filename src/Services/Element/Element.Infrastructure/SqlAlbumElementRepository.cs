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
    public class SqlAlbumElementRepository : IElementRepository<AlbumElementSettings>
    {
        private readonly IOptions<SqlOptions> _options;

        public SqlAlbumElementRepository(IOptions<SqlOptions> options)
        {
            _options = options;
        }

        public async Task<AlbumElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
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
                        element.Album.DisplayName
                    FROM
                        cms.Element
                    INNER JOIN
                        element.Album
                    ON
                        cms.Element.TenantId = element.Album.TenantId AND
                        cms.Element.ElementId = element.Album.ElementId
                    WHERE
                        cms.Element.TenantId = @TenantId AND
                        cms.Element.ElementId = @ElementId
                    SELECT
                        element.AlbumPhoto.AlbumPhotoId AS BlobSetId,
	                    element.AlbumPhoto.ImageUploadId AS ImageBlobId,
	                    element.AlbumPhoto.PreviewImageUploadId AS PreviewImageBlobId,
	                    element.AlbumPhoto.ThumbnailImageUploadId AS ThumbnailImageBlobId,
                        element.AlbumPhoto.Name,
                        element.AlbumPhoto.Description
                    FROM
                        element.AlbumPhoto
                    WHERE
	                    element.AlbumPhoto.TenantId = @TenantId AND
                        element.AlbumPhoto.ElementId = @ElementId
                    ORDER BY
                        element.AlbumPhoto.SortOrder",
                    new
                    {
                        TenantId = tenantId,
                        ElementId = elementId
                    }
                ))
                {
                    AlbumElementSettings settings = await gr.ReadFirstOrDefaultAsync<AlbumElementSettings>();
                    if (settings != null)
                        settings.Photos = await gr.ReadAsync<AlbumPhoto>();
                    return settings;
                }
            }
        }
    }
}
