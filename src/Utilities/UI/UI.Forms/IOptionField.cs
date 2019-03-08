namespace Riverside.Cms.Utilities.UI.Forms
{
    public interface IOptionField : IField, IRequiredField
    {
        string EmptyOptionLabel { get; set; }
        string NullOptionLabel { get; set; }
    }
}
