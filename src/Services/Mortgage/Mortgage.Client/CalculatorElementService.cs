using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Riverside.Cms.Services.Element.Client;

namespace Riverside.Cms.Services.Mortgage.Client
{
    public interface ICalculatorElementService<TElementSettings> : IElementSettingsService<TElementSettings>, IElementViewService<TElementSettings, object> where TElementSettings : IElementSettings
    {
    }

    public abstract class CalculatorElementService<TElementSettings> : ICalculatorElementService<TElementSettings> where TElementSettings : IElementSettings
    {
        private readonly IOptions<MortgageApiOptions> _options;

        public CalculatorElementService(IOptions<MortgageApiOptions> options)
        {
            _options = options;
        }

        public abstract string ElementTypeId { get; }

        public async Task<TElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.MortgageApiBaseUrl}tenants/{tenantId}/elementtypes/{ElementTypeId}/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<TElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<IElementView<TElementSettings, object>> ReadElementViewAsync(long tenantId, long elementId, IPageContext context)
        {
            try
            {
                string uri = $"{_options.Value.MortgageApiBaseUrl}tenants/{tenantId}/elementtypes/{ElementTypeId}/elements/{elementId}/view?pageid={context.PageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ElementView<TElementSettings, object>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
