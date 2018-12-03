﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Riverside.Cms.Applications.Web.Mvc.Models;
using Riverside.Cms.Services.Element.Domain;
using Riverside.Cms.Services.Mortgage.Domain;
using Riverside.Cms.Services.Storage.Domain;

namespace Riverside.Cms.Applications.Web.Mvc.Services
{
    public class ElementServiceFactory : IElementServiceFactory
    {
        // General elements
        private readonly IAlbumElementService _albumElementService;
        private readonly ICarouselElementService _carouselElementService;
        private readonly ICodeSnippetElementService _codeSnippetElementService;
        private readonly IFooterElementService _footerElementService;
        private readonly IFormElementService _formElementService;
        private readonly IForumElementService _forumElementService;
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

        // Mortgage elements
        private readonly IAmortisationCalculatorElementService _amortisationCalculatorElementService;
        private readonly IBorrowCalculatorElementService _borrowCalculatorElementService;
        private readonly IPayCalculatorElementService _payCalculatorElementService;
        private readonly IRentalCalculatorElementService _rentalCalculatorElementService;
        private readonly IStampDutyCalculatorElementService _stampDutyCalculatorElementService;

        public ElementServiceFactory(
            IAlbumElementService albumElementService, ICarouselElementService carouselElementService, ICodeSnippetElementService codeSnippetElementService, IFooterElementService footerElementService, IFormElementService formElementService, IForumElementService forumElementService, IHtmlElementService htmlElementService, ILatestThreadsElementService latestThreadsElementService, INavigationBarElementService navigationBarElementService, IPageHeaderElementService pageHeaderElementService, IPageListElementService pageListElementService, IShareElementService shareElementService, ISocialBarElementService socialBarElementService, ITableElementService tableElementService, ITagCloudElementService tagCloudElementService, ITestimonialElementService testimonialElementService,
            IAmortisationCalculatorElementService amortisationCalculatorElementService, IBorrowCalculatorElementService borrowCalculatorElementService, IPayCalculatorElementService payCalculatorElementService, IRentalCalculatorElementService rentalCalculatorElementService, IStampDutyCalculatorElementService stampDutyCalculatorElementService
        )
        {
            // General elements
            _albumElementService = albumElementService;
            _carouselElementService = carouselElementService;
            _codeSnippetElementService = codeSnippetElementService;
            _footerElementService = footerElementService;
            _formElementService = formElementService;
            _forumElementService = forumElementService;
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

            // Mortgage elements
            _amortisationCalculatorElementService = amortisationCalculatorElementService;
            _borrowCalculatorElementService = borrowCalculatorElementService;
            _payCalculatorElementService = payCalculatorElementService;
            _rentalCalculatorElementService = rentalCalculatorElementService;
            _stampDutyCalculatorElementService = stampDutyCalculatorElementService;
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
                // General elements
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

                case "484192d1-5a4f-496f-981b-7e0120453949":
                    return GetElementViewModelAsync(tenantId, elementId, context, _forumElementService.ReadElementViewAsync);

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

                // Mortgage elements
                case "fb7e757d-905c-4ab1-828f-c39baabe55a6":
                    return GetElementViewModelAsync(tenantId, elementId, context, _amortisationCalculatorElementService.ReadElementViewAsync);

                case "8373bdac-6f80-4c5e-8e27-1b3a9c92cd7c":
                    return GetElementViewModelAsync(tenantId, elementId, context, _borrowCalculatorElementService.ReadElementViewAsync);

                case "f03aa333-4a52-4716-aa1b-1d0d6a31dc15":
                    return GetElementViewModelAsync(tenantId, elementId, context, _payCalculatorElementService.ReadElementViewAsync);

                case "eec21fac-d185-45ee-a8dd-6032679697b1":
                    return GetElementViewModelAsync(tenantId, elementId, context, _rentalCalculatorElementService.ReadElementViewAsync);

                case "9c167ed6-1caf-4b2c-8de1-2586d247e28e":
                    return GetElementViewModelAsync(tenantId, elementId, context, _stampDutyCalculatorElementService.ReadElementViewAsync);

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
