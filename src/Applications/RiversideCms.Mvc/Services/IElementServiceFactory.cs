using System;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Element.Client;
using Riverside.Cms.Services.Storage.Client;

namespace RiversideCms.Mvc.Services
{
    public interface IElementServiceFactory
    {
        Task<IElementView> GetElementViewAsync(long tenantId, Guid elementTypeId, long elementId, PageContext context);
        Task<BlobContent> GetElementBlobContentAsync(long tenantId, Guid elementTypeId, long elementId, long blobSetId, PageImageType imageType);
    }
}
