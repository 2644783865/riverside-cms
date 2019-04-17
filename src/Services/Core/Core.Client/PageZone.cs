using System.Collections.Generic;

namespace Riverside.Cms.Services.Core.Client
{
    public class PageZone
    {
        public long PageZoneId { get; set; }
        public long MasterPageZoneId { get; set; }

        public IEnumerable<PageZoneElement> PageZoneElements { get; set; }
    }
}
