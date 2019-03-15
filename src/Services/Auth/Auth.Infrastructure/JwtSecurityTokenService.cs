using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Riverside.Cms.Services.Auth.Domain;
using Riverside.Cms.Services.Core.Domain;

namespace Riverside.Cms.Services.Auth.Infrastructure
{
    /// <summary>
    /// Credit:
    /// http://jasonwatmore.com/post/2019/01/08/aspnet-core-22-role-based-authorization-tutorial-with-example-api
    /// https://www.carlrippon.com/asp-net-core-web-api-multi-tenant-jwts
    /// </summary>
    public class JwtSecurityTokenService : ISecurityTokenService
    {
        private readonly IOptions<JwtOptions> _options;

        public JwtSecurityTokenService(IOptions<JwtOptions> options)
        {
            _options = options;
        }

        public byte[] GetTenantSecurityKey(long tenantId)
        {
            string securityKey = string.Format(_options.Value.SecurityKey, tenantId);
            return Encoding.ASCII.GetBytes(securityKey);
        }

        public string GenerateToken(IUserIdentity identity)
        {
            // Get list of claims for specified user
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, identity.Alias),
                new Claim(ClaimTypes.Email, identity.Email),
                new Claim(ClaimTypes.GroupSid, identity.TenantId.ToString()),
                new Claim(ClaimTypes.Sid, identity.UserId.ToString()),
                new Claim(ClaimTypes.Version, "1")
            };
            foreach (string role in identity.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            // Generate a JWT token
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(GetTenantSecurityKey(identity.TenantId)), SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            token.Header.Add("kid", identity.TenantId.ToString());
            return tokenHandler.WriteToken(token);
        }
    }
}
