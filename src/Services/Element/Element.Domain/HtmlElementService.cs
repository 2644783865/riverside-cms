using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Services.Element.Domain
{
    public class HtmlBlob
    {
        public long HtmlBlobId { get; set; }
        public long ImageBlobId { get; set; }
        public long PreviewImageBlobId { get; set; }
        public long ThumbnailImageBlobId { get; set; }
    }

    public class HtmlElementSettings : ElementSettings
    {
        public string Html { get; set; }
        public IEnumerable<HtmlBlob> Blobs { get; set; }
    }

    public class HtmlContentImage
    {
        public long BlobId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
    }

    public class HtmlElementContent
    {
        public string FormattedHtml { get; set; }
        public IDictionary<long, HtmlContentImage> Images { get; set; }
    }

    public interface IHtmlElementService : IElementSettingsService<HtmlElementSettings>, IElementViewService<HtmlElementSettings, HtmlElementContent>, IElementStorageService
    {
    }

    public class HtmlElementService : IHtmlElementService
    {
        private readonly IElementRepository<HtmlElementSettings> _elementRepository;
        private readonly IStorageService _storageService;

        private const string HtmlImagePath = "elements/html/{0}";

        public HtmlElementService(IElementRepository<HtmlElementSettings> elementRepository, IStorageService storageService)
        {
            _elementRepository = elementRepository;
            _storageService = storageService;
        }

        public Task<HtmlElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        private string ReplaceKeywords(string html)
        {
            return html.Replace("%YEAR%", DateTime.UtcNow.Year.ToString());
        }

        private string CorrectImageHtml(Guid elementTypeId, string imageHtml, IDictionary<long, HtmlContentImage> imagesByHtmlBlobId)
        {
            string[] imageHtmlParts = imageHtml.Split('/');
            string htmlBlobIdAndQueryString = imageHtmlParts[4];
            int indexOfQueryString = htmlBlobIdAndQueryString.IndexOf("?");
            long htmlBlobId = Convert.ToInt64(htmlBlobIdAndQueryString.Substring(0, indexOfQueryString));

            if (!imagesByHtmlBlobId.ContainsKey(htmlBlobId) || imagesByHtmlBlobId[htmlBlobId] == null)
                return "[[]]";
            // TODO: Add width, heigth, alt in here
            return $"[[IMG:{imagesByHtmlBlobId[htmlBlobId].BlobId}]]";
        }

        private string CorrectImagesHtml(Guid elementTypeId, string html, IDictionary<long, HtmlContentImage> imagesByHtmlBlobId)
        {
            StringBuilder sb = new StringBuilder();
            int currentIndex = 0;
            int imgStartIndex = 0; 
            while (imgStartIndex != -1)
            {
                imgStartIndex = html.IndexOf("<img src=\"/elements/", currentIndex);
                if (imgStartIndex == -1)
                {
                    // Image not found
                    string appendText = html.Substring(currentIndex);
                    sb.Append(appendText);
                }
                else
                {
                    // Image found
                    string appendText = html.Substring(currentIndex, imgStartIndex - currentIndex);
                    sb.Append(appendText);
                    int imgStopIndex = html.IndexOf(">", imgStartIndex) + 1;
                    string imgHtml = html.Substring(imgStartIndex, imgStopIndex - imgStartIndex);
                    imgHtml = CorrectImageHtml(elementTypeId, imgHtml, imagesByHtmlBlobId);
                    sb.Append(imgHtml);
                    currentIndex = imgStopIndex;
                }
            }
            return sb.ToString();
        }

        private string FormatHtml(Guid elementTypeId, string html, IDictionary<long, HtmlContentImage> imagesByHtmlBlobId)
        {
            html = ReplaceKeywords(html);
            html = CorrectImagesHtml(elementTypeId, html, imagesByHtmlBlobId);
            return html;
        }

        private HtmlContentImage GetHtmlImageFromBlobId(long blobId, IDictionary<long, BlobImage> blobsById)
        {
            if (!blobsById.ContainsKey(blobId))
                return null;

            BlobImage blobImage = blobsById[blobId];

            return new HtmlContentImage
            {
                BlobId = blobImage.BlobId,
                Width = blobImage.Width,
                Height = blobImage.Height,
                Name = blobImage.Name
            };
        }

        public async Task<IElementView<HtmlElementSettings, HtmlElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            HtmlElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            // Retrieve all preview image blobs
            IEnumerable<long> blobIds = settings.Blobs.Select(b => b.PreviewImageBlobId);
            IEnumerable<Blob> blobs = await _storageService.ListBlobsAsync(tenantId, blobIds);
            IDictionary<long, BlobImage> blobsById = blobs.ToDictionary(b => b.BlobId, b => (BlobImage)b);
            // TODO: Check what if mismatch GetHtmlImageFromBlobId returns NULLs?
            IDictionary<long, HtmlContentImage> imagesByHtmlBlobId = settings.Blobs.ToDictionary(h => h.HtmlBlobId, h => GetHtmlImageFromBlobId(h.PreviewImageBlobId, blobsById));
            IDictionary<long, HtmlContentImage> imagesByBlobId = settings.Blobs.Select(h => GetHtmlImageFromBlobId(h.PreviewImageBlobId, blobsById)).ToDictionary(h => h.BlobId, h => h);

            // Format HTML
            string formattedHtml = FormatHtml(settings.ElementTypeId, settings.Html, imagesByHtmlBlobId);

            HtmlElementContent content = new HtmlElementContent
            {
                FormattedHtml = formattedHtml,
                Images = imagesByBlobId
            };

            return new ElementView<HtmlElementSettings, HtmlElementContent>
            {
                Settings = settings,
                Content = content
            };
        }

        private long? GetBlobId(HtmlBlob htmlBlob, PageImageType pageImageType)
        {
            switch (pageImageType)
            {
                case PageImageType.Original:
                    return htmlBlob.ImageBlobId;

                case PageImageType.Preview:
                    return htmlBlob.PreviewImageBlobId;

                case PageImageType.Thumbnail:
                    return htmlBlob.ThumbnailImageBlobId;

                default:
                    return null;
            }
        }

        public async Task<BlobContent> ReadBlobContentAsync(long tenantId, long elementId, long elementBlobId, PageImageType imageType)
        {
            HtmlElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            HtmlBlob blob = settings.Blobs.Where(b => b.HtmlBlobId == elementBlobId).FirstOrDefault();
            if (blob == null)
                return null;

            long? blobId = GetBlobId(blob, imageType);
            if (blobId == null)
                return null;

            return await _storageService.ReadBlobContentAsync(tenantId, blobId.Value, string.Format(HtmlImagePath, elementId));
        }
    }
}
