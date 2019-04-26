using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Storage.Domain
{
    public interface IStorageRepository
    {
        Task<IEnumerable<Blob>> SearchBlobsAsync(long tenantId, string path);
        Task<long> CreateBlobAsync(long tenantId, Blob blob);
        Task<long> CreateBlobImageAsync(long tenantId, BlobImage blob);
        Task<Blob> ReadBlobAsync(long tenantId, long blobId);
        Task CommitBlobAsync(long tenantId, long blobId, DateTime updated);
        Task DeleteBlobAsync(long tenantId, long blobId);
        Task<IEnumerable<Blob>> ListBlobsAsync(long tenantId, IEnumerable<long> blobIds);
    }
}
