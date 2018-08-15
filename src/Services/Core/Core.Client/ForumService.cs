using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Riverside.Cms.Services.Core.Client
{
    public class ForumService : IForumService
    {
        private readonly IOptions<CoreApiOptions> _options;

        public ForumService(IOptions<CoreApiOptions> options)
        {
            _options = options;
        }

        public async Task<IEnumerable<ForumThread>> ListLatestThreadsAsync(long tenantId, long? parentPageId, bool recursive, IEnumerable<long> tagIds, int pageSize)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/threads?recursive={recursive.ToString().ToLower()}" +
                    (tagIds != null && tagIds.Count() > 0 ? $"&tagids={string.Join(",", tagIds)}" : string.Empty) +
                    (parentPageId.HasValue ? $"&parentpageid={parentPageId.Value}" : string.Empty);
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<IEnumerable<ForumThread>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }
    }
}
