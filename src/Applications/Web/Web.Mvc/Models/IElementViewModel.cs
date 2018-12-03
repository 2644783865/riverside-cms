using System;
using Riverside.Cms.Services.Element.Domain;

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
