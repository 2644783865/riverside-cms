using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Riverside.Cms.Services.Element.Client
{
    public class TableElementSettings : ElementSettings
    {
        public string DisplayName { get; set; }
        public string Preamble { get; set; }
        public bool ShowHeaders { get; set; }
        public string Rows { get; set; }
    }

    public class TableElementContent
    {
        public IEnumerable<string> Headers { get; set; }
        public IEnumerable<IEnumerable<string>> Rows { get; set; }
    }

    public interface ITableElementService : IElementSettingsService<TableElementSettings>, IElementViewService<TableElementSettings, TableElementContent>
    {
    }

    public class TableElementService : ITableElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public TableElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<TableElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/252ca19c-d085-4e0d-b70b-da3e1098f51b/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<TableElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<IElementView<TableElementSettings, TableElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/252ca19c-d085-4e0d-b70b-da3e1098f51b/elements/{elementId}/view?pageid={context.PageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ElementView<TableElementSettings, TableElementContent>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
