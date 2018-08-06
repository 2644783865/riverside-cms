using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Element.Client
{
    public class ElementView : IElementView
    {
        public IElementSettings Settings { get; set; }
        public IElementContent Content { get; set; }
    }

    public class ElementView<TSettings, TContent> : ElementView, IElementView<TSettings, TContent> where TSettings : IElementSettings where TContent : IElementContent
    {
        TSettings IElementView<TSettings, TContent>.Settings
        {
            get
            {
                return (TSettings)base.Settings;
            }
            set
            {
                base.Settings = value;
            }
        }

        TContent IElementView<TSettings, TContent>.Content
        {
            get
            {
                return (TContent)base.Content;
            }
            set
            {
                base.Content = value;
            }
        }
    }
}
