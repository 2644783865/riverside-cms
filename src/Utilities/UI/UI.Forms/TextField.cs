namespace Riverside.Cms.Utilities.UI.Forms
{
    public class TextField : ITextField
    {
        public string Id { get; set; }
        public string Label { get; set; }

        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public string LengthMessage { get; set; }

        public string Pattern { get; set; }

        public bool Required { get; set; }
        public string RequiredMessage { get; set; }

        public int? Rows { get; set; }
    }
}
