using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Auth.Domain
{
    public interface IUserSession
    {
        IUserIdentity Identity { get; set; }
        IUserSecurity Security { get; set; }
    }
}
