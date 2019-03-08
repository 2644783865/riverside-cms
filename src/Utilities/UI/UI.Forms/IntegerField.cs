namespace Riverside.Cms.Utilities.UI.Forms
{
    public class IntegerField : IIntegerField
    {
        public string Id { get; set; }
        public string Label { get; set; }

        public int? Min { get; set; }
        public int? Max { get; set; }
        public string MinMaxMessage { get; set; }

        public string InvalidMessage { get; set; }
    }
}
