using System.IO;

namespace Riverside.Cms.Utilities.Drawing.ImageAnalysis
{
    public interface IImageAnalysisService
    {
        ImageMetadata GetImageMetadata(Stream stream);
        Stream ResizeImage(Stream stream, ResizeOptions options);
    }
}
