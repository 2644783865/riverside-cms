using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Element.Client
{
    public interface IElementView
    {
        IElementSettings Settings { get; set; }
        IElementContent Content { get; set; }
    }

    public interface IElementView<TSettings, TContent> : IElementView where TSettings : IElementSettings where TContent : IElementContent
    {
        new TSettings Settings { get; set; }
        new TContent Content { get; set; }
    }
}
