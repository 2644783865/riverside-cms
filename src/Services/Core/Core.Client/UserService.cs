using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Riverside.Cms.Services.Storage.Client;

namespace Riverside.Cms.Services.Core.Client
{
    public class UserService : IUserService
    {
        private readonly IOptions<CoreApiOptions> _options;

        public UserService(IOptions<CoreApiOptions> options)
        {
            _options = options;
        }

        public async Task<User> ReadUserAsync(long tenantId, long userId)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/users/{userId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<User>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<BlobContent> ReadUserImageAsync(long tenantId, long userId, UserImageType userImageType)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/users/{userId}/images/{userImageType.ToString().ToLower()}";
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(uri);
                    return new BlobContent
                    {
                        Name = response.Content.Headers.ContentDisposition.FileName,
                        Type = response.Content.Headers.ContentType.MediaType,
                        Stream = await response.Content.ReadAsStreamAsync()
                    };
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }

        public async Task<IEnumerable<User>> ListUsersAsync(long tenantId, IEnumerable<long> userIds)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}tenants/{tenantId}/users" +
                    (userIds != null && userIds.Count() > 0 ? $"?userIds={string.Join(",", userIds)}" : string.Empty);
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<IEnumerable<User>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }
    }
}
