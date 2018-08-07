using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Client
{
    public enum Language
    {
        Apache,
        Bash,
        CSharp,
        CPlusPlus,
        Css,
        CoffeeScript,
        Diff,
        Html,
        Http,
        Ini,
        Json,
        Java,
        JavaScript,
        Makefile,
        Markdown,
        Nginx,
        ObjectiveC,
        Php,
        Perl,
        Python,
        Ruby,
        Sql,
        Xml
    }

    public class CodeSnippetElementSettings : ElementSettings
    {
        public string Code { get; set; }
        public Language Language { get; set; }
    }

    public interface ICodeSnippetElementService : IElementSettingsService<CodeSnippetElementSettings>, IElementViewService<CodeSnippetElementSettings, object>
    {
    }

    public class CodeSnippetElementService : ICodeSnippetElementService
    {
        private readonly IOptions<ElementApiOptions> _options;

        public CodeSnippetElementService(IOptions<ElementApiOptions> options)
        {
            _options = options;
        }

        public async Task<CodeSnippetElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/5401977d-865f-4a7a-b416-0a26305615de/elements/{elementId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<CodeSnippetElementSettings>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }

        public async Task<IElementView<CodeSnippetElementSettings, object>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            try
            {
                string uri = $"{_options.Value.ElementApiBaseUrl}tenants/{tenantId}/elementtypes/5401977d-865f-4a7a-b416-0a26305615de/elements/{elementId}/view?pageid={context.PageId}";
                using (HttpClient httpClient = new HttpClient())
                {
                    string json = await httpClient.GetStringAsync(uri);
                    return JsonConvert.DeserializeObject<ElementView<CodeSnippetElementSettings, object>>(json);
                }
            }
            catch (Exception ex)
            {
                throw new ElementClientException("Element API failed", ex);
            }
        }
    }
}
