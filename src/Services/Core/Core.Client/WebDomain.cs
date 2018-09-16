using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Core.Client
{
    public class WebDomain
    {
        public long TenantId { get; set; }
        public long DomainId { get; set; }
        public string Url { get; set; }
        public string RedirectUrl { get; set; }
    }
}
