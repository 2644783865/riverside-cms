using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using Riverside.Cms.Services.Auth.Domain;
using Riverside.Cms.Services.Core.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Riverside.Cms.Services.Auth.Infrastructure
{
    public class CookieAuthenticationProviderService : IAuthenticationProviderService
    {
        private IHttpContextAccessor _httpContextAccessor;

        public CookieAuthenticationProviderService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public AuthenticationSession GetLoggedOnSession()
        {
            if (_httpContextAccessor.HttpContext.User == null || !_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return null;

            ClaimsPrincipal principal = _httpContextAccessor.HttpContext.User;

            return new AuthenticationSession
            {
                Persist = Convert.ToBoolean(principal.Claims.Where(c => c.Type == "Persist").Select(c => c.Value).FirstOrDefault()),
                Identity = new UserIdentity
                {
                    Alias = principal.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault(),
                    Email = principal.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).FirstOrDefault(),
                    UserId = Convert.ToInt64(principal.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).FirstOrDefault()),
                    TenantId = Convert.ToInt64(principal.Claims.Where(c => c.Type == "TenantId").Select(c => c.Value).FirstOrDefault()),
                    Roles = principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)
                }
            };
        }

        public void Logoff()
        {
            _httpContextAccessor.HttpContext.SignOutAsync().Wait();
        }

        public void Logon(AuthenticationSession session)
        {
            AuthenticationProperties properties = null;

            if (session.Persist)
                properties = new AuthenticationProperties { IsPersistent = true };
            else
                properties = new AuthenticationProperties { ExpiresUtc = DateTime.UtcNow.AddMinutes(30), IsPersistent = true };

            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, session.Identity.Alias));
            identity.AddClaim(new Claim(ClaimTypes.Email, session.Identity.Email));
            identity.AddClaim(new Claim("TenantId", session.Identity.TenantId.ToString()));
            identity.AddClaim(new Claim("UserId", session.Identity.UserId.ToString()));
            identity.AddClaim(new Claim("Persist", session.Persist.ToString()));
            identity.AddClaim(new Claim("Version", "1"));
            foreach (string role in session.Identity.Roles)
                identity.AddClaim(new Claim(ClaimTypes.Role, role));

            _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties).Wait();
        }
    }
}
