using System.Net;
using FluentValidation;
using WebApplication5.Models;
using WebApplication5.Models.DTO;
using WebApplication5.Repository.IRepository;

namespace WebApplication5.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository authRepository;
    private readonly IValidator<LoginRequestDTO> validatorLogin;
    private readonly IValidator<RegistrationRequestDTO> validatorRegistration;

    public AuthService(IAuthRepository authRepository,
        IValidator<LoginRequestDTO> validatorLogin,
        IValidator<RegistrationRequestDTO> validatorRegistration)
    {
        this.authRepository = authRepository;
        this.validatorLogin = validatorLogin;
        this.validatorRegistration = validatorRegistration;
    }
    
    public async Task<ApiResponse> AuthServiceRegister(RegistrationRequestDTO registrationRequestDto)
    {
        ApiResponse response;

        var validatorResult = await validatorRegistration.ValidateAsync(registrationRequestDto);
        if (!validatorResult.IsValid)
        {
            response = new()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add("Request from validation is not correct");
            return response;
        }
        
        if (!authRepository.IsUserUnique(registrationRequestDto.UserName))
        {
            response = new()
            {
                StatusCode = HttpStatusCode.Conflict,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add("Username is not available");
            return response;
        }

        UserDTO result = await authRepository.Register(registrationRequestDto);
        if (result is null || string.IsNullOrEmpty(result.UserName))
        {
            response = new()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add("Request is not correct");
            return response;
        }
        
        response = new()
        {
            StatusCode = HttpStatusCode.OK,
            Result = result,
            IsSuccess = true,
        };
        return response;
    }

    public async Task<ApiResponse> AuthServiceLogin(LoginRequestDTO loginRequestDto)
    {
        ApiResponse response;

        var validateRes = await validatorLogin.ValidateAsync(loginRequestDto);
        if (!validateRes.IsValid)
        {
            response = new()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add("Request from validation is not correct");
            return response;
        }

        LoginResponseDTO result = await authRepository.Login(loginRequestDto);
        if (result is null)
        {
            return new ApiResponse()
            {
                IsSuccess = false,
                Result = null,
                StatusCode = HttpStatusCode.Unauthorized
            };
        }
        
        response = new()
        {
            StatusCode = HttpStatusCode.OK,
            Result = result,
            IsSuccess = true,
        };

        return response;
    }
}