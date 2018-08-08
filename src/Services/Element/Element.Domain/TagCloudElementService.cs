using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Domain
{
    public class TagCloudElementSettings : ElementSettings
    {
        public bool Recursive { get; set; }
        public long? PageId { get; set; }
        public string DisplayName { get; set; }
        public string NoTagsMessage { get; set; }
    }

    public class TagCloudElementContent
    {
    }

    public interface ITagCloudElementService : IElementSettingsService<TagCloudElementSettings>, IElementViewService<TagCloudElementSettings, TagCloudElementContent>
    {
    }

    public class TagCloudElementService : ITagCloudElementService
    {
        private readonly IElementRepository<TagCloudElementSettings> _elementRepository;

        public TagCloudElementService(IElementRepository<TagCloudElementSettings> elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public Task<TagCloudElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<IElementView<TagCloudElementSettings, TagCloudElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            TagCloudElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);

            TagCloudElementContent content = new TagCloudElementContent();

            return new ElementView<TagCloudElementSettings, TagCloudElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
