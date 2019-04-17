using System;

namespace Riverside.Cms.Services.Core.Client
{
    public class PageZoneElement
    {
        public long PageZoneElementId { get; set; }

        public Guid ElementTypeId { get; set; }
        public long ElementId { get; set; }

        public long? MasterPageZoneElementId { get; set; }
    }
}
