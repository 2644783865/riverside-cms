using System;

namespace Riverside.Cms.Services.Core.Domain
{
    public class ElementDefinition
    {
        public long TenantId { get; set; }
        public Guid ElementTypeId { get; set; }
        public long ElementId { get; set; }

        public string Name { get; set; }
    }
}
