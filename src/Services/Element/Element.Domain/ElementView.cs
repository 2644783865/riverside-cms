using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Element.Domain
{
    public class ElementView<TSettings, TContent> : IElementView<TSettings, TContent> where TSettings : IElementSettings where TContent : IElementContent
    {
        public TSettings Settings { get; set; }
        public TContent Content { get; set; }
    }
}
