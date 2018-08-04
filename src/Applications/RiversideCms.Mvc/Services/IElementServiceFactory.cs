using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riverside.Cms.Services.Element.Client;
using RiversideCms.Mvc.Models;

namespace RiversideCms.Mvc.Services
{
    public interface IElementServiceFactory
    {
        Task<IElementView> GetElementViewAsync(long tenantId, Guid elementTypeId, long elementId, long pageId, IEnumerable<long> tagIds);
    }
}
