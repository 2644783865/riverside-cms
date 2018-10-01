using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Riverside.Cms.Applications.Web.Mvc.Models;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Element.Client;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Applications.Web.Mvc.Services
{
    public class ElementServiceFactory : IElementServiceFactory
    {
        private readonly IAlbumElementService _albumElementService;
        private readonly ICarouselElementService _carouselElementService;
        private readonly ICodeSnippetElementService _codeSnippetElementService;
        private readonly IFooterElementService _footerElementService;
        private readonly IFormElementService _formElementService;
        private readonly IHtmlElementService _htmlElementService;
        private readonly ILatestThreadsElementService _latestThreadsElementService;
        private readonly INavigationBarElementService _navigationBarElementService;
        private readonly IPageHeaderElementService _pageHeaderElementService;
        private readonly IPageListElementService _pageListElementService;
        private readonly IShareElementService _shareElementService;
        private readonly ISocialBarElementService _socialBarElementService;
        private readonly ITableElementService _tableElementService;
        private readonly ITagCloudElementService _tagCloudElementService;
        private readonly ITestimonialElementService _testimonialElementService;

        public ElementServiceFactory(IAlbumElementService albumElementService, ICarouselElementService carouselElementService, ICodeSnippetElementService codeSnippetElementService, IFooterElementService footerElementService, IFormElementService formElementService, IHtmlElementService htmlElementService, ILatestThreadsElementService latestThreadsElementService, INavigationBarElementService navigationBarElementService, IPageHeaderElementService pageHeaderElementService, IPageListElementService pageListElementService, IShareElementService shareElementService, ISocialBarElementService socialBarElementService, ITableElementService tableElementService, ITagCloudElementService tagCloudElementService, ITestimonialElementService testimonialElementService)
        {
            _albumElementService = albumElementService;
            _carouselElementService = carouselElementService;
            _codeSnippetElementService = codeSnippetElementService;
            _footerElementService = footerElementService;
            _formElementService = formElementService;
            _htmlElementService = htmlElementService;
            _latestThreadsElementService = latestThreadsElementService;
            _navigationBarElementService = navigationBarElementService;
            _pageHeaderElementService = pageHeaderElementService;
            _pageListElementService = pageListElementService;
            _shareElementService = shareElementService;
            _socialBarElementService = socialBarElementService;
            _tableElementService = tableElementService;
            _tagCloudElementService = tagCloudElementService;
            _testimonialElementService = testimonialElementService;
        }

        private async Task<IElementViewModel> GetElementViewModelAsync<TSettings, TContent>(long tenantId, long elementId, IPageContext context, Func<long, long, IPageContext, Task<IElementView<TSettings, TContent>>> func) where TSettings : IElementSettings
        {
            IElementView view = await func(tenantId, elementId, context);
            if (view == null)
                return null;
            return new ElementViewModel<TSettings, TContent>
            {
                View = view,
                Context = context
            };
        }

        public Task<IElementViewModel> GetElementViewModelAsync(long tenantId, Guid elementTypeId, long elementId, IPageContext context)
        {
            switch (elementTypeId.ToString())
            {
                case "b539d2a4-52ae-40d5-b366-e42447b93d15":
                    return GetElementViewModelAsync(tenantId, elementId, context, _albumElementService.ReadElementViewAsync);

                case "aacb11a0-5532-47cb-aab9-939cee3d5175":
                    return GetElementViewModelAsync(tenantId, elementId, context, _carouselElementService.ReadElementViewAsync);

                case "5401977d-865f-4a7a-b416-0a26305615de":
                    return GetElementViewModelAsync(tenantId, elementId, context, _codeSnippetElementService.ReadElementViewAsync);

                case "f1c2b384-4909-47c8-ada7-cd3cc7f32620":
                    return GetElementViewModelAsync(tenantId, elementId, context, _footerElementService.ReadElementViewAsync);

                case "eafbd5ab-8c98-4edc-b8e1-42f5e8bfe2dc":
                    return GetElementViewModelAsync(tenantId, elementId, context, _formElementService.ReadElementViewAsync);

                case "c92ee4c4-b133-44cc-8322-640e99c334dc":
                    return GetElementViewModelAsync(tenantId, elementId, context, _htmlElementService.ReadElementViewAsync);

                case "f9557287-ba01-48e3-9ab4-e2f4831933d0":
                    return GetElementViewModelAsync(tenantId, elementId, context, _latestThreadsElementService.ReadElementViewAsync);

                case "a94c34c0-1a4c-4c91-a669-2f830cf1ea5f":
                    return GetElementViewModelAsync(tenantId, elementId, context, _navigationBarElementService.ReadElementViewAsync);

                case "1cbac30c-5deb-404e-8ea8-aabc20c82aa8":
                    return GetElementViewModelAsync(tenantId, elementId, context, _pageHeaderElementService.ReadElementViewAsync);

                case "61f55535-9f3e-4ef5-96a2-bc84d648842a":
                    return GetElementViewModelAsync(tenantId, elementId, context, _pageListElementService.ReadElementViewAsync);

                case "cf0d7834-54fb-4a6e-86db-0f238f8b1ac1":
                    return GetElementViewModelAsync(tenantId, elementId, context, _shareElementService.ReadElementViewAsync);

                case "4e6b936d-e8a1-4ff2-9576-9f9b78f82895":
                    return GetElementViewModelAsync(tenantId, elementId, context, _socialBarElementService.ReadElementViewAsync);

                case "252ca19c-d085-4e0d-b70b-da3e1098f51b":
                    return GetElementViewModelAsync(tenantId, elementId, context, _tableElementService.ReadElementViewAsync);

                case "b910c231-7dbd-4cad-92ef-775981e895b4":
                    return GetElementViewModelAsync(tenantId, elementId, context, _tagCloudElementService.ReadElementViewAsync);

                case "eb479ac9-8c79-4fae-817a-e77fd3dbf05b":
                    return GetElementViewModelAsync(tenantId, elementId, context, _testimonialElementService.ReadElementViewAsync);

                default:
                    return Task.FromResult<IElementViewModel>(null);
            }
        }

        public Task<BlobContent> GetElementBlobContentAsync(long tenantId, Guid elementTypeId, long elementId, long blobSetId, string blobLabel)
        {
            switch (elementTypeId.ToString())
            {
                case "b539d2a4-52ae-40d5-b366-e42447b93d15":
                    return _albumElementService.ReadBlobContentAsync(tenantId, elementId, blobSetId, blobLabel);

                case "aacb11a0-5532-47cb-aab9-939cee3d5175":
                    return _carouselElementService.ReadBlobContentAsync(tenantId, elementId, blobSetId, blobLabel);

                case "c92ee4c4-b133-44cc-8322-640e99c334dc":
                    return _htmlElementService.ReadBlobContentAsync(tenantId, elementId, blobSetId, blobLabel);

                default:
                    return Task.FromResult<BlobContent>(null);
            }
        }

        private TRequest DeserializeJson<TRequest>(string json) => JsonConvert.DeserializeObject<TRequest>(json);

        public async Task<object> PerformElementActionAsync(long tenantId, Guid elementTypeId, long elementId, string requestJson, IPageContext context)
        {
            switch (elementTypeId.ToString())
            {
                case "eafbd5ab-8c98-4edc-b8e1-42f5e8bfe2dc":
                    return await _formElementService.PerformElementActionAsync(tenantId, elementId, DeserializeJson<FormElementActionRequest>(requestJson), context);

                default:
                    return null;
            }
        }
    }
}
