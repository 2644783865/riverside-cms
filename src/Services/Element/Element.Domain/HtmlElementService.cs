using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Services.Element.Domain
{
    public class HtmlBlobSet
    {
        public long BlobSetId { get; set; }
        public long ImageBlobId { get; set; }
        public long PreviewImageBlobId { get; set; }
        public long ThumbnailImageBlobId { get; set; }
    }

    public class HtmlElementSettings : ElementSettings
    {
        public string Html { get; set; }
        public IEnumerable<HtmlBlobSet> BlobSets { get; set; }
    }

    public class HtmlPreviewImage
    {
        public long BlobSetId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
    }

    public class HtmlPreviewImageOverride
    {
        public long BlobSetId { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Name { get; set; }
    }

    public class HtmlElementContent
    {
        public string FormattedHtml { get; set; }
        public IDictionary<long, HtmlPreviewImage> Images { get; set; }
    }

    public interface IHtmlElementService : IElementSettingsService<HtmlElementSettings>, IElementViewService<HtmlElementSettings, HtmlElementContent>, IElementStorageService
    {
    }

    public class HtmlElementService : IHtmlElementService
    {
        private readonly IElementRepository<HtmlElementSettings> _elementRepository;
        private readonly IStorageService _storageService;

        private const string HtmlImagePath = "elements/html/{0}";

        private const string OriginalBlobLabel = "original";
        private const string ThumbnailBlobLabel = "thumbnail";
        private const string PreviewBlobLabel = "preview";

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

        /// <summary>
        /// Gets attribute value from an HTML element.
        /// </summary>
        /// <param name="html">Html, e.g. "<img src='/elements/775/uploads/397?t=636576020049621023' width='45' height='56' alt='Car photo' />"</param>
        /// <param name="attribute">Name of attribute to retrieve, e.g. "width".</param>
        /// <returns>Attribute value.</returns>
        private string GetAttribute(string html, string attribute)
        {
            // Find beginning of attribute
            string attributeStartText = $"{attribute}=\"";
            int attributeStartIndex = html.IndexOf(attributeStartText);
            if (attributeStartIndex == -1)
                return null;

            // Find end of attribute value
            string attributeStopText = "\"";
            int attributeStopIndex = html.IndexOf(attributeStopText, attributeStartIndex + attributeStartText.Length);
            if (attributeStopIndex == -1)
                return null;

            // Return attribute value that is between double quotes
            return html.Substring(attributeStartIndex + attributeStartText.Length, attributeStopIndex - attributeStartIndex - attributeStartText.Length);
        }

        /// <summary>
        /// Converts HTML image tag details into an JSON object.
        /// </summary>
        /// <param name="imageHtml">Html, e.g. "<img src='/elements/775/uploads/397?t=636576020049621023' width='45' height='56' alt='Car photo' />"</param>
        /// <returns>JSON representation of object within opening and closing double square brackets.</returns>
        private string ReplaceImageWithJson(string imageHtml, IDictionary<long, HtmlPreviewImage> previewImagesByBlobSetId)
        {
            // Get HTML image tag attributes
            string src = GetAttribute(imageHtml, "src");
            string width = GetAttribute(imageHtml, "width");
            string height = GetAttribute(imageHtml, "height");
            string alt = GetAttribute(imageHtml, "alt");

            // At a minimum we need src attribute to determine blob identifier
            if (src == null)
                return "[[{}]]";

            // Src attribute determines HTML blob identifier
            string[] srcParts = src.Split('/');
            if (srcParts.Length < 5)
                return "[[{}]]";
            string blobSetIdAndQueryString = srcParts[4];
            int indexOfQueryString = blobSetIdAndQueryString.IndexOf("?");
            if (indexOfQueryString == -1)
                return "[[{}]]";
            string blobSetIdText = blobSetIdAndQueryString.Substring(0, indexOfQueryString);
            long blobSetId;
            if (!Int64.TryParse(blobSetIdText, out blobSetId))
                return "[[{}]]";

            // Check that image with HTML blob identifier exists
            if (!previewImagesByBlobSetId.ContainsKey(blobSetId) || previewImagesByBlobSetId[blobSetId] == null)
                return "[[{}]]";

            // Finally, encode image HTML into JSON
            HtmlPreviewImageOverride image = new HtmlPreviewImageOverride
            {
                BlobSetId = blobSetId,
                Name = alt,
                Width = width,
                Height = height
            };
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            };
            return $"[[{JsonConvert.SerializeObject(image, serializerSettings)}]]";
        }

        /// <summary>
        /// Parses HTML, replacing image tags with JSON representations.
        /// </summary>
        /// <param name="html">The original HTML.</param>
        /// <param name="previewImagesByBlobSetId"></param>
        /// <returns></returns>
        private string ReplaceImagesWithJson(string html, IDictionary<long, HtmlPreviewImage> previewImagesByBlobSetId)
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
                    imgHtml = ReplaceImageWithJson(imgHtml, previewImagesByBlobSetId);
                    sb.Append(imgHtml);
                    currentIndex = imgStopIndex;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Formats HTML.
        /// </summary>
        /// <param name="html">Source HTML.</param>
        /// <param name="previewImagesByBlobSetId"></param>
        /// <returns>Reformatted HTML.</returns>
        private string FormatHtml(string html, IDictionary<long, HtmlPreviewImage> previewImagesByBlobSetId)
        {
            html = ReplaceKeywords(html);
            html = ReplaceImagesWithJson(html, previewImagesByBlobSetId);
            return html;
        }

        private HtmlPreviewImage GetHtmlPreviewImageFromHtmlBlob(HtmlBlobSet blobSet, IDictionary<long, BlobImage> blobsById)
        {
            BlobImage blobImage = blobsById[blobSet.PreviewImageBlobId];
            return new HtmlPreviewImage
            {
                BlobSetId = blobSet.BlobSetId,
                Width = blobImage.Width,
                Height = blobImage.Height,
                Name = blobImage.Name
            };
        }

        public async Task<IElementView<HtmlElementSettings, HtmlElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            // Get element settings
            HtmlElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            // Get details of all preview images that may be displayed in the HTML content
            IEnumerable<long> blobIds = settings.BlobSets.Select(s => s.PreviewImageBlobId);
            IEnumerable<Blob> blobs = await _storageService.ListBlobsAsync(tenantId, blobIds);
            IDictionary<long, BlobImage> blobsById = blobs.ToDictionary(b => b.BlobId, b => (BlobImage)b);

            // Construct dictionary containing details of all preview images keyed by blob set identifier
            IDictionary<long, HtmlPreviewImage> previewImagesByHtmlBlobId = settings.BlobSets
                .Where(s => blobsById.ContainsKey(s.PreviewImageBlobId))
                .Select(s => GetHtmlPreviewImageFromHtmlBlob(s, blobsById))
                .ToDictionary(i => i.BlobSetId, i => i);

            // Format HTML
            string formattedHtml = FormatHtml(settings.Html, previewImagesByHtmlBlobId);

            // Construct element content
            HtmlElementContent content = new HtmlElementContent
            {
                FormattedHtml = formattedHtml,
                Images = previewImagesByHtmlBlobId
            };

            // Return element view
            return new ElementView<HtmlElementSettings, HtmlElementContent>
            {
                Settings = settings,
                Content = content
            };
        }

        private long? GetBlobId(HtmlBlobSet blobSet, string blobLabel)
        {
            switch (blobLabel)
            {
                case OriginalBlobLabel:
                    return blobSet.ImageBlobId;

                case PreviewBlobLabel:
                    return blobSet.PreviewImageBlobId;

                case ThumbnailBlobLabel:
                    return blobSet.ThumbnailImageBlobId;

                default:
                    return blobSet.PreviewImageBlobId;
            }
        }

        public async Task<BlobContent> ReadBlobContentAsync(long tenantId, long elementId, long blobSetId, string blobLabel)
        {
            HtmlElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            HtmlBlobSet blobSet = settings.BlobSets.Where(s => s.BlobSetId == blobSetId).FirstOrDefault();
            if (blobSet == null)
                return null;

            long? blobId = GetBlobId(blobSet, blobLabel);
            if (blobId == null)
                return null;

            return await _storageService.ReadBlobContentAsync(tenantId, blobId.Value, string.Format(HtmlImagePath, elementId));
        }
    }
}
