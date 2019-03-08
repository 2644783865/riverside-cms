namespace Riverside.Cms.Utilities.UI.Forms
{
    public interface ILengthField
    {
        int? MinLength { get; set; }
        int? MaxLength { get; set; }
        string LengthMessage { get; set; }
    }
}
