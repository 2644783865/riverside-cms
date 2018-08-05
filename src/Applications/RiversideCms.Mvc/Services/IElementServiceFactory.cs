using System;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;
using RiversideCms.Mvc.Models;

namespace RiversideCms.Mvc.Services
{
    public interface IElementServiceFactory
    {
        Task<IElementView> GetElementViewAsync(long tenantId, Guid elementTypeId, long elementId, PageContext context);
    }
}
