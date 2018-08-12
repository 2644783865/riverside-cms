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
    }

    public interface ILatestThreadsElementService : IElementSettingsService<LatestThreadsElementSettings>, IElementViewService<LatestThreadsElementSettings, LatestThreadsElementContent>
    {
    }

    public class LatestThreadsElementService : ILatestThreadsElementService
    {
        private readonly IElementRepository<LatestThreadsElementSettings> _elementRepository;

        public LatestThreadsElementService(IElementRepository<LatestThreadsElementSettings> elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public Task<LatestThreadsElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<IElementView<LatestThreadsElementSettings, LatestThreadsElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            LatestThreadsElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);

            LatestThreadsElementContent content = new LatestThreadsElementContent();

            return new ElementView<LatestThreadsElementSettings, LatestThreadsElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
