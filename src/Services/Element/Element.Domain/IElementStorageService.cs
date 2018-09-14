using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Storage.Domain;

namespace Riverside.Cms.Services.Element.Domain
{
    public interface IElementStorageService
    {
        Task<BlobContent> ReadBlobContentAsync(long tenantId, long elementId, long blobSetId, string blobLabel);
    }
}
