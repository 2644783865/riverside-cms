namespace Riverside.Cms.Services.Storage.Domain
{
    public class ImageResizeUploadResult : IUploadResult
    {
        public long Size { get; set; }
        public string Name { get; set; }

        public long ImageBlobId { get; set; } 
        public long PreviewImageBlobId { get; set; }
        public long ThumbnailImageBlobId { get; set; }
    }
}
