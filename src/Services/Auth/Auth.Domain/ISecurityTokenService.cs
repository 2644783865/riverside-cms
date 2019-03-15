using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Auth.Domain
{
    public interface ISecurityTokenService
    {
        byte[] GetTenantSecurityKey(long tenantId);
        string GenerateToken(IUserIdentity identity);
    }
}
