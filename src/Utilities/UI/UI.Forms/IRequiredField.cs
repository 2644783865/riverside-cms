namespace Riverside.Cms.Utilities.UI.Forms
{
    public interface IRequiredField
    {
        bool Required { get; set; }
        string RequiredMessage { get; set; }
    }
}
