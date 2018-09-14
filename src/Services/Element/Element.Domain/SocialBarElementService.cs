using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Element.Domain
{
    public class SocialBarElementSettings : ElementSettings
    {
        public string DisplayName { get; set; }
        public string Preamble { get; set; }
        public string TwitterUsername { get; set; }
        public string FacebookUsername { get; set; }
        public string LinkedInCompanyUsername { get; set; }
        public string LinkedInPersonalUsername { get; set; }
        public string InstagramUsername { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string TelephoneNumber1 { get; set; }
        public string TelephoneNumber2 { get; set; }
        public string YouTubeChannelId { get; set; }
    }

    public class SocialBarElementContent
    {
        public string TwitterUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string LinkedInPersonalUrl { get; set; }
        public string LinkedInCompanyUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string YouTubeChannelUrl { get; set; }
    }

    public interface ISocialBarElementService : IElementSettingsService<SocialBarElementSettings>, IElementViewService<SocialBarElementSettings, SocialBarElementContent>
    {
    }

    public class SocialBarElementService : ISocialBarElementService
    {
        private readonly IElementRepository<SocialBarElementSettings> _elementRepository;

        public SocialBarElementService(IElementRepository<SocialBarElementSettings> elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public Task<SocialBarElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<IElementView<SocialBarElementSettings, SocialBarElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            SocialBarElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            SocialBarElementContent content = new SocialBarElementContent
            {
                FacebookUrl = settings.FacebookUsername != null ? string.Format("https://www.facebook.com/{0}", settings.FacebookUsername) : null,
                InstagramUrl = settings.InstagramUsername != null ? string.Format("https://www.instagram.com/{0}", settings.InstagramUsername) : null,
                LinkedInCompanyUrl = settings.LinkedInCompanyUsername != null ? string.Format("https://www.linkedin.com/company/{0}", settings.LinkedInCompanyUsername) : null,
                LinkedInPersonalUrl = settings.LinkedInPersonalUsername != null ? string.Format("https://www.linkedin.com/in/{0}", settings.LinkedInPersonalUsername) : null,
                TwitterUrl = settings.TwitterUsername != null ? string.Format("https://twitter.com/{0}", settings.TwitterUsername) : null,
                YouTubeChannelUrl = settings.YouTubeChannelId != null ? string.Format("https://www.youtube.com/channel/{0}", settings.YouTubeChannelId) : null
            };

            return new ElementView<SocialBarElementSettings, SocialBarElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
