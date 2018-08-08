using System;
using System.Collections.Generic;
using System.Linq;
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

    public class TagCloudTag
    {
        public Tag Tag { get; set; }
        public IEnumerable<Tag> ActionTags { get; set; }
    }

    public class TagCloudPageLink
    {
        public bool Home { get; set; }
        public long PageId { get; set; }
        public string Name { get; set; }
    }

    public class TagCloudElementContent
    {
        public bool TagsToDisplay { get; set; }
        public IEnumerable<TagCloudTag> SelectedTags { get; set; }
        public TagCloudPageLink Page { get; set; }
    }

    public interface ITagCloudElementService : IElementSettingsService<TagCloudElementSettings>, IElementViewService<TagCloudElementSettings, TagCloudElementContent>
    {
    }

    public class TagCloudElementService : ITagCloudElementService
    {
        private readonly IElementRepository<TagCloudElementSettings> _elementRepository;
        private readonly IPageService _pageService;
        private readonly ITagService _tagService;

        public TagCloudElementService(IElementRepository<TagCloudElementSettings> elementRepository, IPageService pageService, ITagService tagService)
        {
            _elementRepository = elementRepository;
            _pageService = pageService;
            _tagService = tagService;
        }

        public Task<TagCloudElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        private async Task<TagCloudPageLink> GetPageLinkAsync(long tenantId, long pageId)
        {
            Page page = await _pageService.ReadPageAsync(tenantId, pageId);
            return new TagCloudPageLink
            {
                Home = !page.ParentPageId.HasValue,
                Name = page.Name,
                PageId = pageId
            };
        }

        public async Task<IElementView<TagCloudElementSettings, TagCloudElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            TagCloudElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);

            TagCloudElementContent content = new TagCloudElementContent();

            long tagCloudPageId = settings.PageId ?? context.PageId;
            content.Page = await GetPageLinkAsync(tenantId, tagCloudPageId);

            IEnumerable<Tag> tags = null;
            if (context.TagIds != null)
                tags = await _tagService.ListTagsAsync(tenantId, context.TagIds);
            else
                tags = new List<Tag>();

            content.SelectedTags = tags.Select(t => new TagCloudTag { Tag = t, ActionTags = tags.Where(t2 => t2.Name != t.Name) });

            content.TagsToDisplay = content.SelectedTags.Any(); // || Model.Content.TaggedList.Count > 0 || Model.Content.RelatedTagList.Count > 0;

            return new ElementView<TagCloudElementSettings, TagCloudElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
