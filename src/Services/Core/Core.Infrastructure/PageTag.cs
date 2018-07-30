using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Core.Infrastructure
{
    public class PageTag
    {
        public long TenantId { get; set; }
        public long PageId { get; set; }
        public long TagId { get; set; }
        public string Name { get; set; }
    }
}
