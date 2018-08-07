using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Element.Client
{
    public class ElementView : IElementView
    {
        public IElementSettings Settings { get; set; }
        public object Content { get; set; }
    }

    public class ElementView<TSettings, TContent> : ElementView, IElementView<TSettings, TContent> where TSettings : IElementSettings
    {
        /// <summary>
        /// Credit: Constructor required so that Json.NET does not throw the error "type is an interface or abstract class and cannot be instantiated"
        /// when deserialising from JSON. See https://stackoverflow.com/a/18147504 for more details.
        /// </summary>
        public ElementView(TSettings settings, TContent content)
        {
            Settings = settings;
            Content = content;
        }

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
