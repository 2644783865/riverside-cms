using System;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Element.Client;

namespace RiversideCms.Mvc.Services
{
    public class ElementServiceFactory : IElementServiceFactory
    {
        private readonly ICodeSnippetElementService _codeSnippetElementService;
        private readonly IFooterElementService _footerElementService;
        private readonly IHtmlElementService _htmlElementService;
        private readonly INavigationBarElementService _navigationBarElementService;
        private readonly IPageHeaderElementService _pageHeaderElementService;
        private readonly IPageListElementService _pageListElementService;
        private readonly IShareElementService _shareElementService;

        public ElementServiceFactory(ICodeSnippetElementService codeSnippetElementService, IFooterElementService footerElementService, IHtmlElementService htmlElementService, INavigationBarElementService navigationBarElementService, IPageHeaderElementService pageHeaderElementService, IPageListElementService pageListElementService, IShareElementService shareElementService)
        {
            _codeSnippetElementService = codeSnippetElementService;
            _footerElementService = footerElementService;
            _htmlElementService = htmlElementService;
            _navigationBarElementService = navigationBarElementService;
            _pageHeaderElementService = pageHeaderElementService;
            _pageListElementService = pageListElementService;
            _shareElementService = shareElementService;
        }

        public async Task<IElementView> GetElementViewAsync(long tenantId, Guid elementTypeId, long elementId, PageContext context)
        {
            switch (elementTypeId.ToString())
            {
                case "5401977d-865f-4a7a-b416-0a26305615de":
                    return await _codeSnippetElementService.ReadElementViewAsync(tenantId, elementId, context);

                case "f1c2b384-4909-47c8-ada7-cd3cc7f32620":
                    return await _footerElementService.ReadElementViewAsync(tenantId, elementId, context);

                case "c92ee4c4-b133-44cc-8322-640e99c334dc":
                    return await _htmlElementService.ReadElementViewAsync(tenantId, elementId, context);

                case "a94c34c0-1a4c-4c91-a669-2f830cf1ea5f":
                    return await _navigationBarElementService.ReadElementViewAsync(tenantId, elementId, context);

                case "1cbac30c-5deb-404e-8ea8-aabc20c82aa8":
                    return await _pageHeaderElementService.ReadElementViewAsync(tenantId, elementId, context);

                case "61f55535-9f3e-4ef5-96a2-bc84d648842a":
                    return await _pageListElementService.ReadElementViewAsync(tenantId, elementId, context);

                case "cf0d7834-54fb-4a6e-86db-0f238f8b1ac1":
                    return await _shareElementService.ReadElementViewAsync(tenantId, elementId, context);

                default:
                    return null;
            }
        }
    }
}
