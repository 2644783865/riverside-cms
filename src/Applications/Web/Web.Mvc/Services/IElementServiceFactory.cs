using System;
using System.Threading.Tasks;
using Riverside.Cms.Applications.Web.Mvc.Models;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Element.Client;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Applications.Web.Mvc.Services
{
    public interface IElementServiceFactory
    {
        Task<IElementViewModel> GetElementViewModelAsync(long tenantId, Guid elementTypeId, long elementId, IPageContext context);
        Task<BlobContent> GetElementBlobContentAsync(long tenantId, Guid elementTypeId, long elementId, long blobSetId, string blobLabel);
        Task<object> PerformElementActionAsync(long tenantId, Guid elementTypeId, long elementId, string requestJson, IPageContext context);
    }
}
