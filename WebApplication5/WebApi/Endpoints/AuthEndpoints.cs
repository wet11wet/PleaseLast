using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;
using WebApplication5.Models;
using WebApplication5.Models.DTO;
using WebApplication5.Repository.IRepository;

namespace WebApplication5.Endpoints;

public static class AuthEndpoints
{
    public static void ConfigureAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/register", Register)
            .WithName("Register")
            .Accepts<RegistrationRequestDTO>("application/json")
            .Produces<ApiResponse>(200)
            .Produces(400)
            .Produces(409);
        
        app.MapPost("/api/login", Login)
            .WithName("Login")
            .Accepts<LoginRequestDTO>("application/json")
            .Produces<LoginResponseDTO>(200)
            .Produces(400)
            .Produces(401);
    }

    private async static Task<IResult> Register(RegistrationRequestDTO registrationRequestDto,
        IAuthRepository authRepository,
        IValidator<RegistrationRequestDTO> validator)
    {
        ApiResponse response;

        var validatorResult = await validator.ValidateAsync(registrationRequestDto);
        if (!validatorResult.IsValid)
        {
            response = new()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add("Request from validation is not correct");
            return Results.BadRequest(response);
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
            return Results.Conflict(response);
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
            return Results.BadRequest(response);
        }
        
        response = new()
        {
            StatusCode = HttpStatusCode.OK,
            Result = result,
            IsSuccess = true,
        };

        return Results.Ok(response);
    }
    
    private async static Task<IResult> Login(LoginRequestDTO loginRequestDto
    ,IValidator<LoginRequestDTO> validator,
    IAuthRepository authRepository)
    {
        ApiResponse response;

        var validateRes = await validator.ValidateAsync(loginRequestDto);
        if (!validateRes.IsValid)
        {
            response = new()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add("Request from validation is not correct");
            return Results.BadRequest(response);
        }

        LoginResponseDTO result = await authRepository.Login(loginRequestDto);
        if (result is null)
        {
            return Results.Unauthorized();
        }
        
        response = new()
        {
            StatusCode = HttpStatusCode.OK,
            Result = result,
            IsSuccess = true,
        };
        return Results.Ok(response);
    }
}