using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Core.Common
{
    public class MultiTenantAttribute : Attribute, IFilterFactory
    {
        private readonly bool _redirect;

        public MultiTenantAttribute(bool redirect = false)
        {
            _redirect = redirect;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            IDomainService domainService = (IDomainService)serviceProvider.GetService(typeof(IDomainService));
            return new MultiTenantInternalAttribute(_redirect, domainService);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public class MultiTenantInternalAttribute : IAsyncActionFilter
        {
            private readonly bool _redirect;

            private readonly IDomainService _domainService;

            public MultiTenantInternalAttribute(bool redirect, IDomainService domainService)
            {
                _redirect = redirect;
                _domainService = domainService;
            }

            private void ProcessInvalidDomain(HttpContext context, string rootUrl, string redirectUrl)
            {
                string path = context.Request.Path.Value;
                if (_redirect)
                    context.Response.Redirect($"{redirectUrl}{path}", true);
                else
                    throw new MultiTenantDomainException("Request executed on invalid domain", $"{rootUrl}{path}", $"{redirectUrl}{path}");
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                // Gets "root URL" of current request. For example, the URI "http://localhost:7823/pages/100" has root URI "http://localhost:7823".
                string rootUrl = string.Format("{0}://{1}", context.HttpContext.Request.Scheme, context.HttpContext.Request.Host);

                // Lookup the domain associated with the root URL
                WebDomain domain = await _domainService.ReadDomainByUrlAsync(rootUrl);

                // Throw error if no domain
                if (domain == null)
                    throw new MultiTenantDomainException("Request executed on invalid domain");

                // Throw error if request executed on wrong domain
                if (domain.RedirectUrl != null)
                    ProcessInvalidDomain(context.HttpContext, rootUrl, domain.RedirectUrl);

                // Put domain into http context items, where it can be accessed later by controllers
                context.RouteData.Values["tenantId"] = domain.TenantId;

                // Execute action
                await next();
            }
        }
    }
}
