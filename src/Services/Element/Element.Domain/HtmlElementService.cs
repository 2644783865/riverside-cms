using System;
using System.Collections.Generic;
using System.Linq;
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

        private string FormatHtml(string html)
        {
            return html.Replace("%YEAR%", DateTime.UtcNow.Year.ToString());
        }

        public async Task<IElementView<HtmlElementSettings, HtmlElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            HtmlElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            HtmlElementContent content = new HtmlElementContent
            {
                FormattedHtml = FormatHtml(settings.Html)
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
