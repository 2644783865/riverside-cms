using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Element.Client
{
    public interface IElementView<TSettings, TContent> where TSettings : IElementSettings where TContent : IElementContent
    {
        TSettings Settings { get; set; }
        TContent Content { get; set; }
    }
}
