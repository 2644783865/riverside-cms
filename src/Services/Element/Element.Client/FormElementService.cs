using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Riverside.Cms.Services.Element.Client
{
    public enum FormFieldType
    {
        TextField,
        MultiLineTextField
    }

    public class FormField
    {
        public long FormFieldId { get; set; }
        public string Label { get; set; }
        public FormFieldType FieldType { get; set; }
        public bool Required { get; set; }
    }

    public class FormElementSettings : ElementSettings
    {
        public string RecipientEmail { get; set; }
        public string SubmitButtonLabel { get; set; }
        public string SubmittedMessage { get; set; }
        public IEnumerable<FormField> Fields { get; set; }
    }

    public class FormFieldValue
    {
        public long FormFieldId { get; set; }
        public string Value { get; set; }
    }

    public class FormElementActionRequest
    {
        public IEnumerable<FormFieldValue> Fields { get; set; }
    }

    public class FormElementActionResponse
    {
        public string Message { get; set; }
    }

    public interface IFormElementService : IElementSettingsService<FormElementSettings>, IElementViewService<FormElementSettings, object>, IElementActionService<FormElementActionRequest, FormElementActionResponse>
    {
    }

    public class FormElementService : IFormElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public FormElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<FormElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/eafbd5ab-8c98-4edc-b8e1-42f5e8bfe2dc/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<FormElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<IElementView<FormElementSettings, object>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/eafbd5ab-8c98-4edc-b8e1-42f5e8bfe2dc/elements/{elementId}/view?pageid={context.PageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ElementView<FormElementSettings, object>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<FormElementActionResponse> PerformElementActionAsync(long tenantId, long elementId, FormElementActionRequest request, PageContext context)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/eafbd5ab-8c98-4edc-b8e1-42f5e8bfe2dc/elements/{elementId}/action?pageid={context.PageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string requestJson = JsonConvert.SerializeObject(request);
                    using (HttpContent content = new StringContent(requestJson, Encoding.UTF8, "application/json"))
                    {
                        using (HttpResponseMessage responseMessage = await httpClient.PostAsync(uri, content))
                        {
                            string responseJson = await responseMessage.Content.ReadAsStringAsync();
                            return JsonConvert.DeserializeObject<FormElementActionResponse>(responseJson);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
