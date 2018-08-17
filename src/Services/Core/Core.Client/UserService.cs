using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Riverside.Cms.Services.Core.Client
{
    public class UserService : IUserService
    {
        private readonly IOptions<CoreApiOptions> _options;

        public UserService(IOptions<CoreApiOptions> options)
        {
            _options = options;
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
