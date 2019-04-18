using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IElementService
    {
        Task<ElementDefinition> ReadElementDefinitionAsync(long tenantId, Guid elementTypeId, long elementId);
        Task<IEnumerable<ElementDefinition>> ListElementDefinitionsAsync(long tenantId, Guid elementTypeId, IEnumerable<long> elementIds);

        Task<IEnumerable<ElementType>> ListElementTypesAsync();
        Task<IEnumerable<ElementType>> ListElementTypesAsync(IEnumerable<Guid> elementTypeIds);
    }
}
