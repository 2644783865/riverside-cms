using System;
using System.Collections.Generic;

namespace Riverside.Cms.Services.Core.Client
{
    public class MasterPageZone
    {
        public long MasterPageZoneId { get; set; }

        public AdminType AdminType { get; set; }
        public ContentType ContentType { get; set; }
        public string Name { get; set; }
        public string BeginRender { get; set; }
        public string EndRender { get; set; }

        public IEnumerable<Guid> ElementTypeIds { get; set; }
        public IEnumerable<MasterPageZoneElement> MasterPageZoneElements { get; set; }
    }
}
