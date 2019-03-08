namespace Riverside.Cms.Utilities.UI.Forms
{
    public interface IRangeField
    {
        int? Min { get; set; }
        int? Max { get; set; }
        string MinMaxMessage { get; set; }
    }
}
