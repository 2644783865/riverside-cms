using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public class HtmlElementContent
    {
        public string FormattedHtml { get; set; }
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

        private string FormatBlobUrl(Guid elementTypeId, string blobUrl)
        {
            // Converts URL like: /elements/775/uploads/408?t=636576020355975145 to /elementtypes/c92ee4c4-b133-44cc-8322-640e99c334dc/elements/775/blobs/408/content?t=636576020049621023
            string[] blobUrlParts = blobUrl.Split('/');
            long elementId = Convert.ToInt64(blobUrlParts[2]);
            string elementBlobIdAndQueryString = blobUrlParts[4];
            int indexOfQueryString = elementBlobIdAndQueryString.IndexOf("?");
            long elementBlobId = Convert.ToInt64(elementBlobIdAndQueryString.Substring(0, indexOfQueryString));
            long t = Convert.ToInt64(elementBlobIdAndQueryString.Substring(indexOfQueryString + 3));
            return $"/elementtypes/{elementTypeId.ToString().ToLower()}/elements/{elementId}/blobs/{elementBlobId}/content?t={t}";
        }

        private string FormatBlobUrls(Guid elementTypeId, string html)
        {
            StringBuilder sb = new StringBuilder();
            int currentIndex = 0;
            int blobUrlStartIndex = 0; 
            while (blobUrlStartIndex != -1)
            {
                blobUrlStartIndex = html.IndexOf("/elements/", currentIndex);
                if (blobUrlStartIndex == -1)
                {
                    // Blob URL not found
                    string appendText = html.Substring(currentIndex);
                    sb.Append(appendText);
                }
                else
                {
                    // Blob URL found
                    string appendText = html.Substring(currentIndex, blobUrlStartIndex - currentIndex);
                    sb.Append(appendText);
                    int blobUrlStopIndex = html.IndexOf("\"", blobUrlStartIndex);
                    string blobUrl = html.Substring(blobUrlStartIndex, blobUrlStopIndex - blobUrlStartIndex);
                    blobUrl = FormatBlobUrl(elementTypeId, blobUrl);
                    sb.Append(blobUrl);
                    currentIndex = blobUrlStopIndex;
                }
            }
            return sb.ToString();
        }

        private string FormatHtml(Guid elementTypeId, string html)
        {
            html = ReplaceKeywords(html);
            html = FormatBlobUrls(elementTypeId, html);
            return html;
        }

        public async Task<IElementView<HtmlElementSettings, HtmlElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            HtmlElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            HtmlElementContent content = new HtmlElementContent
            {
                FormattedHtml = FormatHtml(settings.ElementTypeId, settings.Html)
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
