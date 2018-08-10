using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Riverside.Cms.Services.Core.Client
{
    public class TagService : ITagService
    {
        private readonly IOptions<CoreApiOptions> _options;

        public TagService(IOptions<CoreApiOptions> options)
        {
            _options = options;
        }

        public async Task<Tag> ReadTagAsync(long tenantId, long tagId)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/tags/{tagId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<Tag>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<IEnumerable<Tag>> ListTagsAsync(long tenantId, IEnumerable<long> tagIds)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/tags" +
                    (tagIds != null && tagIds.Count() > 0 ? $"?tagids={string.Join(",", tagIds)}" : string.Empty);
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<IEnumerable<Tag>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<IEnumerable<Tag>> ListTagsAsync(long tenantId, IEnumerable<string> tagNames)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/tags" +
                    (tagNames != null && tagNames.Count() > 0 ? $"?tagnames={string.Join(",", tagNames)}" : string.Empty);
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<IEnumerable<Tag>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<IEnumerable<TagCount>> ListTagCountsAsync(long tenantId, long? parentPageId, bool recursive)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/tags/counts?recursive={recursive.ToString().ToLower()}" +
                    (parentPageId.HasValue ? $"&parentpageid={parentPageId.Value}" : string.Empty);
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<IEnumerable<TagCount>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<IEnumerable<TagCount>> ListRelatedTagCountsAsync(long tenantId, IEnumerable<long> tagIds, long? parentPageId, bool recursive)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/tags/related/counts?recursive={recursive.ToString().ToLower()}" +
                    (tagIds != null && tagIds.Count() > 0 ? $"&tagids={string.Join(",", tagIds)}" : string.Empty) +
                    (parentPageId.HasValue ? $"&parentpageid={parentPageId.Value}" : string.Empty);
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<IEnumerable<TagCount>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }
    }
}
