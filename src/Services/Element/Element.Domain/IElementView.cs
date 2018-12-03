using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Element.Domain
{
    public interface IElementView
    {
        IElementSettings Settings { get; set; }
        object Content { get; set; }
    }

    public interface IElementView<TSettings, TContent> : IElementView where TSettings : IElementSettings
    {
        new TSettings Settings { get; set; }
        new TContent Content { get; set; }
    }
}
