using System;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Element.Client;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Applications.Web.Mvc.Services
{
    public interface IElementServiceFactory
    {
        Task<IElementView> GetElementViewAsync(long tenantId, Guid elementTypeId, long elementId, PageContext context);
        Task<BlobContent> GetElementBlobContentAsync(long tenantId, Guid elementTypeId, long elementId, long blobSetId, string blobLabel);
        Task<object> PerformElementActionAsync(long tenantId, Guid elementTypeId, long elementId, string requestJson, PageContext context);
    }
}
