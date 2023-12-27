using Tangy_Models;

namespace Tangy_Webserver.Serivce.IService
{
    public interface IAuthenticationService
    {
        Task<SignUpResponseDTO> SignUp(SignUpRequestDTO signUpRequestDTO);
        Task<SignInResponseDTO> SignIn(SignInRequestDTO signInRequestDTO);
        Task Logout();
    }
}
