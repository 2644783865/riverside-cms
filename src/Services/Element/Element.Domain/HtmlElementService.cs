using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Domain
{
    public class HtmlElementSettings : ElementSettings
    {
        public string Html { get; set; }
    }

    public class HtmlElementContent
    {
        public string FormattedHtml { get; set; }
    }

    public interface IHtmlElementService : IElementSettingsService<HtmlElementSettings>, IElementViewService<HtmlElementSettings, HtmlElementContent>
    {
    }

    public class HtmlElementService : IHtmlElementService
    {
        private readonly IElementRepository<HtmlElementSettings> _elementRepository;

        public HtmlElementService(IElementRepository<HtmlElementSettings> elementRepository)
        {
            _elementRepository = elementRepository;
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
    }
}
