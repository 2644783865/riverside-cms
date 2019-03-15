using System.Collections.Generic;

namespace Riverside.Cms.Services.Core.Domain
{
    public interface IUserIdentity
    {
        long TenantId { get; set; }
        long UserId { get; set; }
        string Alias { get; set; }
        string Email { get; set; }
        IEnumerable<string> Roles { get; set; }
    }
}
