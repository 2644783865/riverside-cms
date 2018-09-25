using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Element.Domain
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

    public interface IFormElementService : IElementSettingsService<FormElementSettings>, IElementViewService<FormElementSettings, object>
    {
    }

    public class FormElementService : IFormElementService
    {
        private readonly IElementRepository<FormElementSettings> _elementRepository;

        public FormElementService(IElementRepository<FormElementSettings> elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public Task<FormElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<IElementView<FormElementSettings, object>> ReadElementViewAsync(long tenantId, long elementId, PageContext context)
        {
            FormElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            return new ElementView<FormElementSettings, object>
            {
                Settings = settings,
                Content = null
            };
        }
    }
}
