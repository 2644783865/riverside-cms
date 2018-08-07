using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Domain
{
    public class FooterElementSettings : ElementSettings
    {
        public bool ShowLoggedOnUserOptions { get; set; }
        public bool ShowLoggedOffUserOptions { get; set; }
        public string Message { get; set; }
    }

    public class FooterElementContent
    {
        public string FormattedMessage { get; set; }
    }

    public interface IFooterElementService : IElementSettingsService<FooterElementSettings>, IElementViewService<FooterElementSettings, FooterElementContent>
    {
    }

    public class FooterElementService : IFooterElementService
    {
        private readonly IElementRepository<FooterElementSettings> _elementRepository;

        public FooterElementService(IElementRepository<FooterElementSettings> elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public Task<FooterElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        private string FormatMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return null;
            return message.Replace("%YEAR%", DateTime.UtcNow.Year.ToString());
        }

        public async Task<IElementView<FooterElementSettings, FooterElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            FooterElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            FooterElementContent content = new FooterElementContent
            {
                FormattedMessage = FormatMessage(settings.Message)
            };

            return new ElementView<FooterElementSettings, FooterElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
