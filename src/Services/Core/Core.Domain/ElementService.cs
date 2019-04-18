using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public class ElementService : IElementService
    {
        private readonly IElementRepository _elementRepository;

        public ElementService(IElementRepository elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public Task<ElementDefinition> ReadElementDefinitionAsync(long tenantId, Guid elementTypeId, long elementId)
        {
            return _elementRepository.ReadElementDefinitionAsync(tenantId, elementTypeId, elementId);
        }

        public Task<IEnumerable<ElementDefinition>> ListElementDefinitionsAsync(long tenantId, Guid elementTypeId, IEnumerable<long> elementIds)
        {
            return _elementRepository.ListElementDefinitionsAsync(tenantId, elementTypeId, elementIds);
        }

        public Task<IEnumerable<ElementType>> ListElementTypesAsync()
        {
            return _elementRepository.ListElementTypesAsync();
        }
    }
}
