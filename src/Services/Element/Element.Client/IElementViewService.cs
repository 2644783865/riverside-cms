using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Client
{
    public interface IElementViewService<TSettings, TContent> where TSettings : IElementSettings
    {
        Task<IElementView<TSettings, TContent>> ReadElementViewAsync(long tenantId, long elementId, IPageContext context);
    }
}
