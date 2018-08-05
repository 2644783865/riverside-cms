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

    public interface ICodeSnippetElementService : IElementSettingsService<CodeSnippetElementSettings>, IElementContentService<ElementContent>
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

        public async Task<ElementContent> ReadElementContentAsync(long tenantId, long elementId, PageContext context)
        {
            CodeSnippetElementSettings elementSettings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            return new ElementContent
            {
                TenantId = tenantId,
                ElementId = elementId,
                ElementTypeId = elementSettings.ElementTypeId
            };
        }
    }
}
