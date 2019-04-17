using System.Collections.Generic;

namespace Riverside.Cms.Services.Core.Client
{
    public class PageViewZone
    {
        public long MasterPageZoneId { get; set; }

        public string BeginRender { get; set; }
        public string EndRender { get; set; }

        public IEnumerable<PageViewZoneElement> PageViewZoneElements { get; set; }
    }
}
