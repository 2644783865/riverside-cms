using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Storage.Domain;
using Riverside.Cms.Utilities.Text.Formatting;

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
        private readonly IStringUtilities _stringUtilities;

        private const string HtmlImagePath = "elements/html/{0}";

        private const string OriginalBlobLabel = "original";
        private const string ThumbnailBlobLabel = "thumbnail";
        private const string PreviewBlobLabel = "preview";

        public HtmlElementService(IElementRepository<HtmlElementSettings> elementRepository, IStorageService storageService, IStringUtilities stringUtilities)
        {
            _elementRepository = elementRepository;
            _storageService = storageService;
            _stringUtilities = stringUtilities;
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
        /// Formats HTML.
        /// </summary>
        /// <param name="html">Source HTML.</param>
        /// <param name="previewImagesByBlobSetId"></param>
        /// <returns>Reformatted HTML.</returns>
        private string FormatHtml(string html, IDictionary<long, HtmlPreviewImage> previewImagesByBlobSetId)
        {
            html = ReplaceKeywords(html);

            HtmlElementHelper helper = new HtmlElementHelper(previewImagesByBlobSetId);
            html = _stringUtilities.BlockReplace(html, "<img src=\"/elements/", ">", helper.Replace);

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

        public async Task<IElementView<HtmlElementSettings, HtmlElementContent>> ReadElementViewAsync(long tenantId, long elementId, IPageContext context)
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
