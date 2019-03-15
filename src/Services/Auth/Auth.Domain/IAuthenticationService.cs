using System.Threading.Tasks;

namespace Riverside.Cms.Services.Auth.Domain
{
    public interface IAuthenticationService
    {
        Task<IUserSession> AuthenticateAsync(long tenantId, LogonModel model);
    }
}
