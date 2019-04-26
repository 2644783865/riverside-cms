using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

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
                using (Image image = Image.FromStream(stream))
                    metadata = new ImageMetadata { Width = image.Width, Height = image.Height };
                stream.Position = position;
                return metadata;
            }
            catch
            {
                return null;
            }
        }

        private Image ResizeDrawingImageSimple(Image sourceImage, ResizeOptions options)
        {
            Bitmap bitmap = new Bitmap(options.Width, options.Height, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(sourceImage,
                    /* Note: The -1, +2 prevents ringing artifacts on resized image */
                    new Rectangle(-1, -1, options.Width + 2, options.Height + 2),
                    new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                    GraphicsUnit.Pixel);
            }
            return bitmap;
        }

        private Image ResizeDrawingImageMaintainAspect(Image sourceImage, ResizeOptions options)
        {
            double aspectHeightRatio = ((double)options.Height) / ((double)sourceImage.Height);
            double aspectWidthRatio = ((double)options.Width) / ((double)sourceImage.Width);
            double aspectRatio = Math.Min(aspectHeightRatio, aspectWidthRatio);
            int width = (int)(((double)sourceImage.Width) * aspectRatio);
            int height = (int)(((double)sourceImage.Height) * aspectRatio);
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(sourceImage,
                    /* Note: The -1, +2 prevents ringing artifacts on resized image */
                    new Rectangle(-1, -1, width + 2, height + 2),
                    new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                    GraphicsUnit.Pixel);
            }
            return bitmap;
        }

        private Image ResizeDrawingImageCrop(Image sourceImage, ResizeOptions options)
        {
            double cropHeightRatio = ((double)options.Height) / ((double)sourceImage.Height);
            double cropWidthRatio = ((double)options.Width) / ((double)sourceImage.Width);
            double cropRatio = Math.Max(cropHeightRatio, cropWidthRatio);
            int unCroppedWidth = (int)(((double)sourceImage.Width) * cropRatio);
            int unCroppedHeight = (int)(((double)sourceImage.Height) * cropRatio);
            int left = -(unCroppedWidth - options.Width) / 2;
            int top = -(unCroppedHeight - options.Height) / 2;
            Bitmap bitmap = new Bitmap(options.Width, options.Height, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(sourceImage,
                    /* Note: The -1, +2 prevents ringing artifacts on resized image */
                    new Rectangle(left - 1, top - 1, unCroppedWidth + 2, unCroppedHeight + 2),
                    new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                    GraphicsUnit.Pixel);
            }
            return bitmap;
        }

        private Image ResizeDrawingImage(Image sourceImage, ResizeOptions options)
        {
            switch (options.Mode)
            {
                case ResizeMode.MaintainAspect:
                    return ResizeDrawingImageMaintainAspect(sourceImage, options);

                case ResizeMode.Crop:
                    return ResizeDrawingImageCrop(sourceImage, options);

                default: /* ResizeMode.Simple */
                    return ResizeDrawingImageSimple(sourceImage, options);
            }
        }

        public Stream ResizeImage(Stream stream, ResizeOptions options)
        {
            MemoryStream destMemoryStream = null;
            long position = stream.Position;
            using (Image image = Image.FromStream(stream))
            {
                using (Image resizedImage = ResizeDrawingImage(image, options))
                {
                    destMemoryStream = new MemoryStream();
                    resizedImage.Save(destMemoryStream, image.RawFormat);
                    destMemoryStream.Position = 0;
                }
            }
            stream.Position = position;
            return destMemoryStream;
        }
    }
}
