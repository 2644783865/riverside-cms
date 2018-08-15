using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Domain
{
    public class LatestThreadsElementSettings : ElementSettings
    {
        public long? PageId { get; set; }
        public string DisplayName { get; set; }
        public string NoThreadsMessage { get; set; }
        public string Preamble { get; set; }
        public int PageSize { get; set; }
        public bool Recursive { get; set; }
    }

    public class LatestThreadsElementContent
    {
        public IEnumerable<ForumThread> Threads { get; set; }
    }

    public interface ILatestThreadsElementService : IElementSettingsService<LatestThreadsElementSettings>, IElementViewService<LatestThreadsElementSettings, LatestThreadsElementContent>
    {
    }

    public class LatestThreadsElementService : ILatestThreadsElementService
    {
        private readonly IElementRepository<LatestThreadsElementSettings> _elementRepository;
        private readonly IForumService _forumService;

        public LatestThreadsElementService(IElementRepository<LatestThreadsElementSettings> elementRepository, IForumService forumService)
        {
            _elementRepository = elementRepository;
            _forumService = forumService;
        }

        public Task<LatestThreadsElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<IElementView<LatestThreadsElementSettings, LatestThreadsElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            LatestThreadsElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);

            LatestThreadsElementContent content = new LatestThreadsElementContent();

            long latestThreadsPageId = settings.PageId ?? context.PageId;

            content.Threads = await _forumService.ListLatestThreadsAsync(tenantId, latestThreadsPageId, settings.Recursive, context.TagIds, settings.PageSize);

            return new ElementView<LatestThreadsElementSettings, LatestThreadsElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
