using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Utilities.Drawing.ImageAnalysis;

namespace Riverside.Cms.Services.Storage.Domain
{
    public class StorageService : IStorageService
    {
        private readonly IBlobService _blobService;
        private readonly IImageAnalysisService _imageAnalysisService;
        private readonly IStorageRepository _storageRepository;

        private const string UncommittedPath = "uncommitted";

        public StorageService(IBlobService blobService, IImageAnalysisService imageAnalysisService, IStorageRepository storageRepository)
        {
            _blobService = blobService;
            _imageAnalysisService = imageAnalysisService;
            _storageRepository = storageRepository;
        }

        private bool ContentTypeIsImage(string contentType)
        {
            switch (contentType)
            {
                case ContentTypes.Gif:
                case ContentTypes.Jpeg:
                case ContentTypes.Png:
                    return true;

                default:
                    return false;
            }
        }

        private BlobImage GetBlobImage(IBlobContent content)
        {
            long position = content.Stream.Position;
            ImageMetadata metadata = _imageAnalysisService.GetImageMetadata(content.Stream);
            content.Stream.Position = position;
            return new BlobImage
            {
                BlobType = BlobType.Image,
                Height = metadata.Height,
                Width = metadata.Width
            };
        }

        public Task<IEnumerable<Blob>> SearchBlobsAsync(long tenantId, string path)
        {
            if (path == null)
                path = string.Empty;
            return _storageRepository.SearchBlobsAsync(tenantId, path);
        }

        public async Task<long> CreateBlobAsync(long tenantId, IBlobContent content)
        {
            // Construct blob object
            bool isImage = ContentTypeIsImage(content.Type);
            Blob blob = isImage ? GetBlobImage(content) : new Blob() { BlobType = BlobType.Document };
            DateTime utcNow = DateTime.UtcNow;
            blob.ContentType = content.Type;
            blob.Created = utcNow;
            blob.Name = content.Name;
            blob.Path = UncommittedPath;
            blob.Size = (int)content.Stream.Length;
            blob.TenantId = tenantId;
            blob.Updated = utcNow;
            
            // Create blob record and get back newly allocated blob identifier
            blob.BlobId = isImage ? await _storageRepository.CreateBlobImageAsync(tenantId, (BlobImage)blob) : await _storageRepository.CreateBlobAsync(tenantId, blob);

            // Create blob content
            await _blobService.CreateBlobContentAsync(blob, content.Stream);

            // Return newly allocated blob identifier
            return blob.BlobId;
        }

        public async Task<long> ResizeBlobAsync(long tenantId, long sourceBlobId, string path, ResizeOptions options)
        {
            Blob blob = await _storageRepository.ReadBlobAsync(tenantId, sourceBlobId);
            Stream imageStream = await _blobService.ReadBlobContentAsync(blob);

            blob.Path = path;
            Stream resizedImageStream = _imageAnalysisService.ResizeImage(imageStream, options);

            IBlobContent content = new BlobContent { Name = blob.Name, Type = blob.ContentType, Stream = resizedImageStream };

            return await CreateBlobAsync(tenantId, content);
        }

        public Task<Blob> ReadBlobAsync(long tenantId, long blobId)
        {
            return _storageRepository.ReadBlobAsync(tenantId, blobId);
        }

        public async Task<BlobContent> ReadBlobContentAsync(long tenantId, long blobId, string path)
        {
            Blob blob = await _storageRepository.ReadBlobAsync(tenantId, blobId);
            blob.Path = path;
            BlobContent blobContent = new BlobContent
            {
                Name = blob.Name,
                Type = blob.ContentType,
                Stream = await _blobService.ReadBlobContentAsync(blob)
            };
            return blobContent;
        }

        public async Task DeleteBlobAsync(long tenantId, long blobId)
        {
            Blob blob = await _storageRepository.ReadBlobAsync(tenantId, blobId);
            if (blob == null)
                return;
            await _blobService.DeleteBlobContentAsync(blob);
            await _storageRepository.DeleteBlobAsync(tenantId, blobId);
        }

        public Task<IEnumerable<Blob>> ListBlobsAsync(long tenantId, IEnumerable<long> blobIds)
        {
            return _storageRepository.ListBlobsAsync(tenantId, blobIds);
        }
    }
}
