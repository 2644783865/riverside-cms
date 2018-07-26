using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Element.Client
{
    public class ElementContent : IElementContent
    {
        public long TenantId { get; set; }
        public Guid ElementTypeId { get; set; }
        public long ElementId { get; set; }
    }
}
