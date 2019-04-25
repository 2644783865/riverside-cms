using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Riverside.Cms.Utilities.Drawing.ImageAnalysis
{
    public class ImageAnalysisService : IImageAnalysisService
    {
        public ImageMetadata GetImageMetadata(Stream stream)
        {
            try
            {
                ImageMetadata metadata = null;
                long position = stream.Position;
                using (Image<Rgba32> image = Image.Load(stream))
                {
                    metadata = new ImageMetadata
                    {
                        Width = image.Width,
                        Height = image.Height
                    };
                }
                stream.Position = position;
                return metadata;
            }
            catch
            {
                return null;
            }
        }

        public Stream ResizeImage(Stream stream, ResizeOptions options)
        {
            IImageFormat format;
            long position = stream.Position;
            MemoryStream ms = null;
            using (Image<Rgba32> image = Image.Load(stream, out format))
            {
                switch (options.Mode)
                {
                    case ResizeMode.Simple:
                        image.Mutate(x => x.Resize(options.Width, options.Height));
                        break;
                }

                ms = new MemoryStream();
                image.Save(ms, format);
                ms.Position = 0;
            }
            stream.Position = position;
            return ms;
        }
    }
}
