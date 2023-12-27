using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using Tangy_Common;
using Tangy_DataAccess;
using Tangy_Webserver.Helper;

namespace Tangy_Webserver.Service
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthStateProvider(ILocalStorageService localStorage,UserManager<ApplicationUser> usermanager)
        {
            _localStorage = localStorage;
            _userManager = usermanager;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userdetails = await _localStorage.GetItemAsync<string>(SD.Local_UserDetails);
                var applicationUser = new ApplicationUser();
                var user = JsonConvert.DeserializeObject<ApplicationUser>(userdetails);
                if (user == null)
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
                applicationUser = new ApplicationUser()
                {
                    Id = user.Id,
                    Name = user.Email,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber

                };
                var claims = await GetClaims(applicationUser);
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "LocalServerAuth")));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));

            }
        }

        public void NotifyUserLoggedIn(List<Claim> claims)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "LocalServerAuth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            NotifyAuthenticationStateChanged(authState);
        }
        public async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Email),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("Id",user.Id)
            };

            var roles = await _userManager.GetRolesAsync(await _userManager.FindByEmailAsync(user.Email));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
