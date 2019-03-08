using System;

namespace Riverside.Cms.Utilities.UI.Forms
{
    public class OptionAttribute : Attribute
    {
        public Type ResourceType { get; set; }
        public string EmptyOptionResourceName { get; set; }
        public string NullOptionResourceName { get; set; }
    }
}
