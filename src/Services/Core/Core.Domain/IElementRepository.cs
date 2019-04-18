using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IElementRepository
    {
        Task<ElementDefinition> ReadElementDefinitionAsync(long tenantId, Guid elementTypeId, long elementId);
        Task<IEnumerable<ElementDefinition>> ListElementDefinitionsAsync(long tenantId, Guid elementTypeId, IEnumerable<long> elementIds);

        Task<IEnumerable<ElementType>> ListElementTypesAsync();
    }
}
