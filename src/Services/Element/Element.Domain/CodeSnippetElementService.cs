using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Core.Client;

namespace Riverside.Cms.Services.Element.Domain
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

    public interface ICodeSnippetElementService : IElementSettingsService<CodeSnippetElementSettings>, IElementViewService<CodeSnippetElementSettings, ElementContent>
    {
    }

    public class CodeSnippetElementService : ICodeSnippetElementService
    {
        private readonly IElementRepository<CodeSnippetElementSettings> _elementRepository;

        public CodeSnippetElementService(IElementRepository<CodeSnippetElementSettings> elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public Task<CodeSnippetElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<IElementView<CodeSnippetElementSettings, ElementContent>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            CodeSnippetElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            ElementContent content = new ElementContent
            {
                TenantId = tenantId,
                ElementId = elementId,
                ElementTypeId = settings.ElementTypeId
            };

            return new ElementView<CodeSnippetElementSettings, ElementContent>
            {
                Settings = settings,
                Content = content
            };
        }
    }
}
