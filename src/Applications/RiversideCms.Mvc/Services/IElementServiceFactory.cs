using System;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Element.Client;

namespace RiversideCms.Mvc.Services
{
    public interface IElementServiceFactory
    {
        Task<IElementView> GetElementViewAsync(long tenantId, Guid elementTypeId, long elementId, PageContext context);
    }
}
