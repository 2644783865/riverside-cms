using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Auth.Domain
{
    public class UserSession : IUserSession
    {
        public IUserIdentity Identity { get; set; }
        public IUserSecurity Security { get; set; }
    }
}
