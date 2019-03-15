using System.Collections.Generic;

namespace Riverside.Cms.Services.Core.Domain
{
    public class UserIdentity : IUserIdentity
    {
        public long TenantId { get; set; }
        public long UserId { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
