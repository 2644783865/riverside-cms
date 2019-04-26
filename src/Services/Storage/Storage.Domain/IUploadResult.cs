namespace Riverside.Cms.Services.Storage.Domain
{
    public interface IUploadResult
    {
        long Size { get; set; }
        string Name { get; set; }
    }
}
