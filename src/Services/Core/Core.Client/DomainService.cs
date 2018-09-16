using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Riverside.Cms.Services.Core.Client
{
    public class DomainService : IDomainService
    {
        private readonly IOptions<CoreApiOptions> _options;

        public DomainService(IOptions<CoreApiOptions> options)
        {
            _options = options;
        }

        public async Task<WebDomain> ReadDomainByUrlAsync(string url)
        {
            try
            {
                string uri = $"{_options.Value.CoreApiBaseUrl}domains?url={WebUtility.UrlEncode(url)}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<WebDomain>(json);
                }
            }
            catch (Exception ex)
            {
                throw new CoreClientException("Core API failed", ex);
            }
        }
    }
}
