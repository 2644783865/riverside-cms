using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Core.Common
{
    public class MultiTenantDomainException : Exception
    {
        public MultiTenantDomainException() { }
        public MultiTenantDomainException(string message) : base(message) { }
        public MultiTenantDomainException(string message, Exception inner) : base(message, inner) { }

        public MultiTenantDomainException(string message, string fromUrl, string toUrl) : base(message)
        {
            FromUrl = fromUrl;
            ToUrl = toUrl;
        }

        public string FromUrl { get; set; }
        public string ToUrl { get; set; }
    }
}
