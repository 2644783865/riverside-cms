using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public class ForumService : IForumService
    {
        private readonly IForumRepository _forumRepository;

        public ForumService(IForumRepository forumRepository)
        {
            _forumRepository = forumRepository;
        }

        public Task<IEnumerable<ForumThread>> ListLatestThreadsAsync(long tenantId, long? parentPageId, bool recursive, IEnumerable<long> tagIds, int pageSize)
        {
            return _forumRepository.ListLatestThreadsAsync(tenantId, parentPageId, recursive, tagIds, pageSize);
        }
    }
}
