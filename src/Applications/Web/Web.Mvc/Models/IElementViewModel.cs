using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riverside.Cms.Services.Element.Client;

namespace Riverside.Cms.Applications.Web.Mvc.Models
{
    public interface IElementViewModel
    {
        IElementView View { get; set; }
        IPageContext Context { get; set; }
    }

    public interface IElementViewModel<TSettings, TContent> : IElementViewModel where TSettings : IElementSettings
    {
        new IElementView<TSettings, TContent> View { get; set; }
    }
}
