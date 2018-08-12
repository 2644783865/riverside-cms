using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IForumService
    {
        Task<IEnumerable<ForumThread>> ListLatestThreadsAsync(long tenantId, long? parentPageId, bool recursive, IEnumerable<long> tagIds, int pageSize);
    }
}
