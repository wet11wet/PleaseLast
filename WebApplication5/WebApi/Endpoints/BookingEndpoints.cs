using System.Net;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using WebApplication5.Application.Services.Room;
using WebApplication5.Models;
using WebApplication5.Models.DTO;
using WebApplication5.Repository.IRepository;
using WebApplication5.Validations;

namespace WebApplication5.Endpoints;

public static class BookingEndpoints
{
    public static void ConfigureBookingEndpoints(this WebApplication app)
    {
        app.MapGet("/api/room/{id:int}", GetRoom)
            .WithName("GetRoom")
            .Produces<ApiResponse>(200)
            .Produces(404)
            .RequireAuthorization();//mutual

        app.MapPut("/api/book", BookRoom)
            .WithName("BookRoom")
            .Accepts<RoomUpdateDTO>("application/json")
            .Produces<ApiResponse>(200)
            .Produces(404)
            .Produces(409)
            .Produces(400)
            .RequireAuthorization(); //user

        app.MapPost("api/room", CreateRoom)
            .WithName("CreateRoom")
            .Accepts<RoomCreateDTO>("application/json")
            .Produces<ApiResponse>(201)
            .Produces(400)
            .RequireAuthorization("AdminOnly"); //admin

        app.MapDelete("api/room/{id:int}", DeleteRoom)
            .WithName("DeleteRoom")
            .Produces<ApiResponse>(200)
            .Produces(404)
            .RequireAuthorization("AdminOnly"); //admin
    }
    
    private static async Task<IResult> GetRoom(int id, 
        [FromServices] IRoomService roomService)
    {
        var response = await roomService.RoomServiceGet(id);
        
        return Results.Json(response, statusCode:(int)response.StatusCode);
    }
    
    private static async Task<IResult> BookRoom(RoomUpdateDTO updateDto,
        [FromServices] IRoomService roomService)
    {
        var response = await roomService.RoomServiceBook(updateDto);
        return Results.Json(response, statusCode: (int) response.StatusCode);
    }
    
    private static async Task<IResult> CreateRoom(RoomCreateDTO createDto,
        [FromServices] IRoomService roomService)
    {
        var response = await roomService.RoomServiceCreate(createDto);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }
    
    private static async Task<IResult> DeleteRoom(int id,
        [FromServices] IRoomService roomService)
    {
        var response = await roomService.RoomServiceDelete(id);
        return Results.Json(response, statusCode: (int)response.StatusCode);
    }
}