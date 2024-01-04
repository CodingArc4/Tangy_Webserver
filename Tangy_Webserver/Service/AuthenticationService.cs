using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Tangy_Common;
using Tangy_DataAccess;
using Tangy_Models;
using Tangy_Webserver.Serivce.IService;
using Tangy_Webserver.Service;

namespace TangyWeb_Server.Service
{
    public class AuthenticationService: Tangy_Webserver.Serivce.IService.IAuthenticationService
    {
        private readonly ProtectedLocalStorage _localStorageService;       
        private readonly AuthenticationStateProvider _authStateProvider; 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthenticationService(ProtectedLocalStorage localStorageService, AuthenticationStateProvider authStateProvider
            , UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _localStorageService = localStorageService;
            _authStateProvider = authStateProvider;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        public async Task<SignInResponseDTO> SignIn(SignInRequestDTO signInRequestDTO)
        {
            var user = await _userManager.FindByEmailAsync(signInRequestDTO.UserName);
            var result = await _signInManager.CheckPasswordSignInAsync(user, signInRequestDTO.Password, false);

            if (result.Succeeded)
            {
                var claims = await GetClaims(user);

                ((AuthStateProvider)_authStateProvider).NotifyUserLoggedIn(claims);

                var userDTO = new SignInResponseDTO
                {
                    IsAuthSuccessful = true,
                    UserDTO = new UserDTO
                    {
                        Name = user.Name,
                        Id = user.Id,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        ExpiryDate = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30)).ToUnixTimeSeconds()
                    }
                };

                return new SignInResponseDTO { IsAuthSuccessful = true };
            }
            else
            {
                return new SignInResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Invalid Authentication"
                };
            }
        }

        //public async Task<SignInResponseDTO> SignIn(SignInRequestDTO signInRequestDTO)
        //{
        //    var user = await _userManager.FindByEmailAsync(signInRequestDTO.UserName);
        //    var result = await _signInManager.CheckPasswordSignInAsync(user, signInRequestDTO.Password,false);

        //    if (result.Succeeded)
        //    {
        //        var claims = await GetClaims(user);

        //        var authProperties = new AuthenticationProperties
        //        {
        //           ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30)) // Set your desired expiration time
        //        };

        //        // Sign in the user using cookies
        //        await _signInManager.SignInAsync(user, authProperties);

        //        var userDTO = new SignInResponseDTO
        //        {
        //            IsAuthSuccessful = true,
        //            UserDTO = new UserDTO
        //            {
        //                Name = user.Name,
        //                Id = user.Id,
        //                Email = user.Email,
        //                PhoneNumber = user.PhoneNumber,
        //                ExpiryDate = authProperties.ExpiresUtc?.ToUnixTimeSeconds() ?? 0

        //            }
        //        };
        //        //await _localStorageService.SetAsync(SD.Local_UserDetails, userDTO);
        //        ((AuthStateProvider)_authStateProvider).NotifyUserLoggedIn(claims);
        //        return new SignInResponseDTO() { IsAuthSuccessful = true };
        //    }
        //    else
        //    {
        //        return new SignInResponseDTO
        //        {
        //            IsAuthSuccessful = false,
        //            ErrorMessage = "Invalid Authentication"
        //        };
        //    }
        //}

        public async Task<SignUpResponseDTO> SignUp(SignUpRequestDTO signUpRequestDTO)
        {
            if (signUpRequestDTO == null)
            {
                return new SignUpResponseDTO
                {
                    IsRegisterationSuccessful = false,
                    Errors = new List<string> { "Invalid model state" }
                };
            }

            var user = new ApplicationUser
            {
                UserName = signUpRequestDTO.Email,
                Email = signUpRequestDTO.Email,
                Name = signUpRequestDTO.Name,
                PhoneNumber = signUpRequestDTO.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, signUpRequestDTO.Password);

            if (!result.Succeeded)
            {
                return new SignUpResponseDTO
                {
                    IsRegisterationSuccessful = false,
                    Errors = result.Errors.Select(u => u.Description).ToList()
                };
            }

            if (result.Succeeded)
			{

				var roleResult = await _userManager.AddToRoleAsync(user, SD.Role_Customer);
				if (!roleResult.Succeeded)
				{
					return new SignUpResponseDTO()
					{
						IsRegisterationSuccessful = false,
						Errors = result.Errors.Select(u => u.Description)
					};
				}
				return new SignUpResponseDTO() { IsRegisterationSuccessful = true };
			}
            return new SignUpResponseDTO
            {
                IsRegisterationSuccessful = true
            };
        }

        private async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Id", user.Id)
            };

            var roles = await _userManager.GetRolesAsync(await _userManager.FindByEmailAsync(user.Email));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }

        public async Task Logout()
        {
            await _localStorageService.DeleteAsync(SD.Local_UserDetails);
            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
        }
    }
}

