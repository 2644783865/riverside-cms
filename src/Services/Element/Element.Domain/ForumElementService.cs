using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Element.Domain
{
    public class ForumElementSettings : ElementSettings
    {
        public long? OwnerTenantId { get; set; }
        public long? OwnerUserId { get; set; }
        public bool OwnerOnlyThreads { get; set; }
    }

    public class ForumPager
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int PageCount { get; set; }
    }

    public class ForumElementContent
    {
        public ForumPager Pager { get; set; }
        public IEnumerable<ForumThread> Threads { get; set; } 
    }

    public interface IForumElementService : IElementSettingsService<ForumElementSettings>, IElementViewService<ForumElementSettings, ForumElementContent>
    {
    }

    public class ForumElementService : IForumElementService
    {
        private readonly IElementRepository<ForumElementSettings> _elementRepository;

        public ForumElementService(IElementRepository<ForumElementSettings> elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public Task<ForumElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<IElementView<ForumElementSettings, ForumElementContent>> ReadElementViewAsync(long tenantId, long elementId, IPageContext context)
        {
            ForumElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);

            ForumElementContent content = new ForumElementContent
            {
            };

            return new ElementView<ForumElementSettings, ForumElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
