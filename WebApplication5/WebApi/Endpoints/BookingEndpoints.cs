using System.Net;
using AutoMapper;
using FluentValidation;
using Microsoft.OpenApi.Validations;
using WebApplication5.Models;
using WebApplication5.Models.DTO;
using WebApplication5.Repository.IRepository;
using WebApplication5.Validations;

namespace WebApplication5.Endpoints;

public static class BookingEndpoints
{
// Должен быть функционал администратора:
// 1 создать комнату, 2 удалить,
// 3 получить (У комнаты есть класс, стоимость, какое-то описание)(у админа просто больше инфы)
    
// Так же функционал пользователя:
// 1 он может забронировать комнату,
// 2 получить информацию по бронированию, (дать айди комнаты и понять забронирована или нет)
// нельзя забронировать комнату,
// которую забронировал другой пользователь.

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
        IRoomRepository roomRepo, 
        IMapper mapper,
        HttpContext httpContext)
    {
        ApiResponse response = new ApiResponse()
        {
            IsSuccess = false,
            StatusCode = HttpStatusCode.NotFound
        };
        
        var room = await roomRepo.GetAsync(id);
        if (room == null)
        {
            response.ErrorMessages.Add( "id is not valid");
            return Results.NotFound(response);
        }

        var isAdmin = httpContext.User.IsInRole("Admin");

        if (isAdmin)
        {
            AdminViewDTO result1 = mapper.Map<AdminViewDTO>(room);
            response.Result = result1;
            response.StatusCode = HttpStatusCode.OK;
            response.IsSuccess = true;
        }
        else
        {
            RoomDTO result2 = mapper.Map<RoomDTO>(room);
            response.Result = result2;
            response.StatusCode = HttpStatusCode.OK;
            response.IsSuccess = true;
        }
        
        return Results.Ok(response);
    }
    
    private static async Task<IResult> BookRoom(RoomUpdateDTO updateDto,
        IMapper mapper,
        IRoomRepository roomRepo,
        IValidator<RoomUpdateDTO> validator)
    {
        ApiResponse response;
        
        var validationRes = await validator.ValidateAsync(updateDto);

        if (!validationRes.IsValid)
        {
            response = new()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add("Necessary field should not be null");
            return Results.BadRequest(response);
        }

        var room = await roomRepo.GetAsync(updateDto.RoomName);
        if (room == null)
        {
            response = new()
            {
                StatusCode = HttpStatusCode.NotFound,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add( "RoomName is not valid");
            return Results.NotFound(response);
        }
        if (!room.IsAvailable)
        {
            response = new()
            {
                StatusCode = HttpStatusCode.Conflict,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add("Room is not available");
            return Results.Conflict(response);
        }
        
        room.IsAvailable = false;

        await roomRepo.UpdateAsync(room);
        await roomRepo.SaveAsync();

        RoomDTO result = mapper.Map<RoomDTO>(room);
        response = new ApiResponse()
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Results.Ok(response);
    }
    
    private static async Task<IResult> CreateRoom(RoomCreateDTO createDto,
        IMapper mapper,
        IValidator<RoomCreateDTO> validator,
        IRoomRepository roomRepo)
    {
        ApiResponse response;

        var validationRes = await validator.ValidateAsync(createDto);
        if (!validationRes.IsValid)
        {
            response = new()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add("Necessary field should not be null");
            return Results.BadRequest(response);
        }

        var roomExisting = await roomRepo.GetAsync(createDto.RoomName);
        if (roomExisting is not null)
        {
            response = new()
            {
                StatusCode = HttpStatusCode.Conflict,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add("Room already exists");
            return Results.Conflict(response);
        }

        var room = mapper.Map<Room>(createDto);
        await roomRepo.CreateAsync(room);
        await roomRepo.SaveAsync();

        RoomDTO result = mapper.Map<RoomDTO>(room);
        
        response = new()
        {
            StatusCode = HttpStatusCode.Created,
            Result = result,
            IsSuccess = true,
        };
        
        return Results.CreatedAtRoute("GetRoom", new {id = room.Id}, result);
    }
    
    private static async Task<IResult> DeleteRoom(int id,
        IRoomRepository roomRepo)
    {
        ApiResponse response;

        var room = await roomRepo.GetAsync(id);
        if (room == null)
        {
            response = new()
            {
                StatusCode = HttpStatusCode.NotFound,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add( "ID is not valid");
            return Results.NotFound(response);
        }
        
        await roomRepo.DeleteAsync(room);
        await roomRepo.SaveAsync();
        
        response = new()
        {
            StatusCode = HttpStatusCode.OK,
            Result = null,
            IsSuccess = true,
        };
        
        return Results.Ok(response);
    }
}