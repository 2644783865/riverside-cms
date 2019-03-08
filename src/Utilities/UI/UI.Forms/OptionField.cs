namespace Riverside.Cms.Utilities.UI.Forms
{
    public class OptionField : IOptionField
    {
        public string Id { get; set; }
        public string Label { get; set; }

        public bool Required { get; set; }
        public string RequiredMessage { get; set; }

        public string EmptyOptionLabel { get; set; }
        public string NullOptionLabel { get; set; }
    }
}
