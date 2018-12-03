using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Applications.Web.Mvc.Controllers
{
    public class ValidateDomainAttribute : Attribute, IFilterFactory
    {
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            IDomainService domainService = (IDomainService)serviceProvider.GetService(typeof(IDomainService));
            return new ValidateDomainInternalAttribute(domainService);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public class ValidateDomainInternalAttribute : IAsyncActionFilter
        {
            private readonly IDomainService _domainService;

            public ValidateDomainInternalAttribute(IDomainService domainService)
            {
                _domainService = domainService;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                // Gets "root URL" of current request. For example, the URI "http://localhost:7823/pages/100" has root URI "http://localhost:7823".
                string rootUrl = string.Format("{0}://{1}", context.HttpContext.Request.Scheme, context.HttpContext.Request.Host);

                // Lookup the domain associated with the root URL
                WebDomain domain = await _domainService.ReadDomainByUrlAsync(rootUrl);

                // Put domain into http context items, where it can be accessed later by controllers
                context.HttpContext.Items["riverside-cms-domain"] = domain;

                // Continue or redirect
                if (domain.RedirectUrl != null)
                    context.HttpContext.Response.Redirect($"{domain.RedirectUrl}{context.HttpContext.Request.Path.Value}", true);
                else
                    await next();
            }
        }
    }
}
