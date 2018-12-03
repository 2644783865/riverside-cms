using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riverside.Cms.Services.Element.Domain;

namespace Riverside.Cms.Applications.Web.Mvc.Models
{
    public class ElementViewModel : IElementViewModel
    {
        public IElementView View { get; set; }
        public IPageContext Context { get; set; }
    }

    public class ElementViewModel<TSettings, TContent> : ElementViewModel, IElementViewModel<TSettings, TContent> where TSettings : IElementSettings
    {
        IElementView<TSettings, TContent> IElementViewModel<TSettings, TContent>.View
        {
            get
            {
                return (IElementView<TSettings, TContent>)base.View;
            }
            set
            {
                base.View = value;
            }
        }
    }
}