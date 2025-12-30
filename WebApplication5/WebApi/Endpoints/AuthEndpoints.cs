using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using WebApplication5.Application.Services.Auth;
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
        [FromServices] IAuthService authService)
    {
        var response = await authService.AuthServiceRegister(registrationRequestDto);

        return Results.Json(response, statusCode: (int)response.StatusCode);
    }
    
    private async static Task<IResult> Login(LoginRequestDTO loginRequestDto,
        [FromServices] IAuthService authService)
    {
        var response = await authService.AuthServiceLogin(loginRequestDto);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }
}