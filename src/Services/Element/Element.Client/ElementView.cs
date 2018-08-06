using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Element.Client
{
    public class ElementView<TSettings, TContent> : IElementView<TSettings, TContent> where TSettings : IElementSettings where TContent : IElementContent
    {
        public TSettings Settings { get; set; }
        public TContent Content { get; set; }
    }
}
