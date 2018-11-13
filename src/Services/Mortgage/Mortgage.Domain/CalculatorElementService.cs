using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Riverside.Cms.Services.Element.Domain;

namespace Riverside.Cms.Services.Mortgage.Domain
{
    public interface ICalculatorElementService<TElementSettings> : IElementSettingsService<TElementSettings>, IElementViewService<TElementSettings, object> where TElementSettings : IElementSettings
    {
    }

    public class CalculatorElementService<TElementSettings> : ICalculatorElementService<TElementSettings> where TElementSettings : IElementSettings
    {
        private readonly IElementRepository<TElementSettings> _elementRepository;

        public CalculatorElementService(IElementRepository<TElementSettings> elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public Task<TElementSettings> ReadElementSettingsAsync(long tenantId, long elementId)
        {
            return _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
        }

        public async Task<IElementView<TElementSettings, object>> ReadElementViewAsync(long tenantId, long elementId, IPageContext context)
        {
            TElementSettings settings = await _elementRepository.ReadElementSettingsAsync(tenantId, elementId);
            if (settings == null)
                return null;

            return new ElementView<TElementSettings, object>
            {
                Settings = settings,
                Content = null
            };
        }
    }
}
