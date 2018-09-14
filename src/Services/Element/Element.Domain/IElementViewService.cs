using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Element.Domain
{
    public interface IElementViewService<TSettings, TContent> where TSettings : IElementSettings
    {
        Task<IElementView<TSettings, TContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context);
    }
}
