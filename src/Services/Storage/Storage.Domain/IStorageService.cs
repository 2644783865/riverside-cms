using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Utilities.Drawing.ImageAnalysis;

namespace Riverside.Cms.Services.Storage.Domain
{
    public interface IStorageService
    {
        Task<IEnumerable<Blob>> SearchBlobsAsync(long tenantId, string path);
        Task<long> CreateBlobAsync(long tenantId, IBlobContent content);
        Task<long> ResizeBlobAsync(long tenantId, long sourceBlobId, string path, ResizeOptions options);
        Task<Blob> ReadBlobAsync(long tenantId, long blobId);
        Task<BlobContent> ReadBlobContentAsync(long tenantId, long blobId, string path);
        Task CommitBlobAsync(long tenantId, long blobId, string path);
        Task DeleteBlobAsync(long tenantId, long blobId, string path);
        Task<IEnumerable<Blob>> ListBlobsAsync(long tenantId, IEnumerable<long> blobIds);
    }
}
