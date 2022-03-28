using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using StarApp1.Models;
using System.Security.Claims;

namespace StarApp1.Services
{
    public interface IAuthHandler
    {
        AuthenticationProperties GetAuthProp(int timeout,LoginViewModel model);
        ClaimsIdentity GetClaimsIdentity(LoginViewModel model);
    }
    public class AuthHandler : IAuthHandler
    {
        public AuthenticationProperties GetAuthProp(int timeout,LoginViewModel model)
        {
            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,

                ExpiresUtc = DateTimeOffset.Now.AddMinutes(timeout),

                IsPersistent = model.RememberMe,

                IssuedUtc = DateTime.UtcNow,

            };
            return authProperties;
        }

        public ClaimsIdentity GetClaimsIdentity(LoginViewModel model)
        {
            List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Name, model.UserName) };

            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return claimsIdentity;
        }

        
        
    }
}
