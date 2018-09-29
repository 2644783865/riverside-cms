using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Riverside.Cms.Services.Element.Client
{
    public class Testimonial
    {
        public long TestimonialId { get; set; }
        public string Comment { get; set; }
        public string Author { get; set; }
        public string AuthorTitle { get; set; }
        public string Date { get; set; }
    }

    public class TestimonialElementSettings : ElementSettings
    {
        public string DisplayName { get; set; }
        public string Preamble { get; set; }
        public IEnumerable<Testimonial> Testimonials { get; set; }
    }

    public interface ITestimonialElementService : IElementSettingsService<TestimonialElementSettings>, IElementViewService<TestimonialElementSettings, object>
    {
    }

    public class TestimonialElementService : ITestimonialElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public TestimonialElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<TestimonialElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/eb479ac9-8c79-4fae-817a-e77fd3dbf05b/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<TestimonialElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<IElementView<TestimonialElementSettings, object>> ReadElementViewAsync(long tenantId, long elementId, IPageContext context)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/eb479ac9-8c79-4fae-817a-e77fd3dbf05b/elements/{elementId}/view?pageid={context.PageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ElementView<TestimonialElementSettings, object>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
