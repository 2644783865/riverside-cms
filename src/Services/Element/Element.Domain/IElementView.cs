using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Element.Domain
{
    public interface IElementView<TSettings, TContent> where TSettings : IElementSettings
    {
        TSettings Settings { get; set; }
        TContent Content { get; set; }
    }
}
