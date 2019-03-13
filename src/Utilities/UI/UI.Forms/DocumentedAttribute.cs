using System;

namespace Riverside.Cms.Utilities.UI.Forms
{
    public class DocumentedAttribute : Attribute
    {
        public Type ResourceType { get; set; }
        public string HelpMessageResourceName { get; set; }
    }
}
