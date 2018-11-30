using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Services.Core.Domain
{
    public class UserIdentity
    {
        public long TenantId { get; set; }
        public long UserId { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
