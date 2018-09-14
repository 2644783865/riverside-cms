using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riverside.Cms.Services.Storage.Domain;

namespace Riverside.Cms.Services.Element.Domain
{
    public class AlbumPhoto
    {
        public long BlobSetId { get; set; }
        public long ImageBlobId { get; set; }
        public long PreviewImageBlobId { get; set; }
        public long ThumbnailImageBlobId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class AlbumElementSettings : ElementSettings
    {
        public string DisplayName { get; set; }
        public IEnumerable<AlbumPhoto> Photos { get; set; }
    }

    public class AlbumContentPhoto
    {
        public long BlobSetId { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public int PreviewWidth { get; set; }
        public int PreviewHeight { get; set; }
    }

    public class AlbumElementContent
    {
        public IDictionary<long, AlbumContentPhoto> Photos { get; set; }
    }

    public interface IAlbumElementService : IElementSettingsService<AlbumElementSettings>, IElementViewService<AlbumElementSettings, AlbumElementContent>, IElementStorageService
    {
    }

    public class AlbumElementService : IAlbumElementService
    {
        private readonly IElementRepository<AlbumElementSettings> _elementRepository;
        private readonly IStorageService _storageService;

        private const string AlbumImagePath = "elements/albums/{0}";

        private const string OriginalBlobLabel = "original";
        private const string ThumbnailBlobLabel = "thumbnail";
        private const string PreviewBlobLabel = "preview";

        public AlbumElementService(IElementRepository<AlbumElementSettings> elementRepository, IStorageService storageService)
        {
            _elementRepository = elementRepository;
            _storageService = storageService;
        }

        public Task<AlbumElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<IElementView<AlbumElementSettings, AlbumElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            // Get element settings
            AlbumElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            // Get details of thumbnail and preview images
            IEnumerable<long> thumbnailImageBlobIds = settings.Photos.Select(p => p.ThumbnailImageBlobId);
            IEnumerable<long> previewImageBlobIds = settings.Photos.Select(p => p.PreviewImageBlobId);
            IEnumerable<Blob> thumbnailImageBlobs = await _storageService.ListBlobsAsync(tenantId, thumbnailImageBlobIds);
            IEnumerable<Blob> previewImageBlobs = await _storageService.ListBlobsAsync(tenantId, previewImageBlobIds);
            IDictionary<long, BlobImage> thumbnailImageBlobsById = thumbnailImageBlobs.ToDictionary(b => b.BlobId, b => (BlobImage)b);
            IDictionary<long, BlobImage> previewImageBlobsById = previewImageBlobs.ToDictionary(b => b.BlobId, b => (BlobImage)b);

            // Construct list of photos for album content
            IDictionary<long, AlbumContentPhoto> photos = settings
                .Photos
                .Where(p => thumbnailImageBlobsById.ContainsKey(p.ThumbnailImageBlobId) && previewImageBlobsById.ContainsKey(p.PreviewImageBlobId))
                .Select(p => new AlbumContentPhoto
                {
                    BlobSetId = p.BlobSetId,
                    ThumbnailWidth = thumbnailImageBlobsById[p.ThumbnailImageBlobId].Width,
                    ThumbnailHeight = thumbnailImageBlobsById[p.ThumbnailImageBlobId].Height,
                    PreviewWidth = previewImageBlobsById[p.PreviewImageBlobId].Width,
                    PreviewHeight = previewImageBlobsById[p.PreviewImageBlobId].Height
                })
                .ToDictionary(p => p.BlobSetId, p => p);

            // Construct element content
            AlbumElementContent content = new AlbumElementContent
            {
                Photos = photos
            };

            // Return element view
            return new ElementView<AlbumElementSettings, AlbumElementContent>
            {
                Settings = settings,
                Content = content
            };
        }

        private long? GetBlobId(AlbumPhoto photo, string blobLabel)
        {
            switch (blobLabel)
            {
                case OriginalBlobLabel:
                    return photo.ImageBlobId;

                case PreviewBlobLabel:
                    return photo.PreviewImageBlobId;

                case ThumbnailBlobLabel:
                    return photo.ThumbnailImageBlobId;

                default:
                    return photo.PreviewImageBlobId;
            }
        }

        public async Task<BlobContent> ReadBlobContentAsync(long tenantId, long elementId, long blobSetId, string blobLabel)
        {
            AlbumElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            AlbumPhoto photo = settings.Photos.Where(p => p.BlobSetId == blobSetId).FirstOrDefault();
            if (photo == null)
                return null;

            long? blobId = GetBlobId(photo, blobLabel);
            if (blobId == null)
                return null;

            return await _storageService.ReadBlobContentAsync(tenantId, blobId.Value, string.Format(AlbumImagePath, elementId));
        }
    }
}
