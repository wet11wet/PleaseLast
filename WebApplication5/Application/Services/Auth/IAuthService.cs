using WebApplication5.Models;
using WebApplication5.Models.DTO;

namespace WebApplication5.Application.Services.Auth;

public interface IAuthService
{
    Task<ApiResponse> AuthServiceRegister(RegistrationRequestDTO registrationRequestDto);
    
    Task<ApiResponse> AuthServiceLogin(LoginRequestDTO loginRequestDto);
}