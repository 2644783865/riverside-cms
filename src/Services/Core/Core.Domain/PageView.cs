using System.Collections.Generic;

namespace Riverside.Cms.Services.Core.Domain
{
    public class PageView
    {
        public long TenantId { get; set; }
        public long MasterPageId { get; set; }
        public long PageId { get; set; }

        public long? PreviewImageBlobId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string BeginRender { get; set; }
        public string EndRender { get; set; }

        public string GoogleSiteVerification { get; set; }

        public IEnumerable<PageViewZone> PageViewZones { get; set; }
    }
}
