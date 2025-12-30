using System.Net;
using AutoMapper;
using FluentValidation;
using WebApplication5.Models;
using WebApplication5.Models.DTO;
using WebApplication5.Repository.IRepository;

namespace WebApplication5.Application.Services.Room;

public class RoomService : IRoomService
{
    private readonly IMapper mapper;
    private readonly IRoomRepository roomRepo;
    private readonly IValidator<RoomUpdateDTO> validatorUpdate;
    private readonly IValidator<RoomCreateDTO> validatorCreate;
    private readonly IHttpContextAccessor httpContextAccessor;
    
    public RoomService(
        IMapper mapper,
        IRoomRepository roomRepo,
        IValidator<RoomUpdateDTO> validatorUpdate,
        IValidator<RoomCreateDTO> validatorCreate,
        IHttpContextAccessor httpContextAccessor)
    {
        this.roomRepo = roomRepo;
        this.validatorCreate = validatorCreate;
        this.validatorUpdate = validatorUpdate;
        this.mapper = mapper;
        this.httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ApiResponse> RoomServiceGet(int id)
    {
        var response = new ApiResponse
        {
            IsSuccess = false,
            StatusCode = HttpStatusCode.NotFound
        };

        var room = await roomRepo.GetAsync(id);
        if (room == null)
        {
            response.ErrorMessages.Add("Id is not valid");
            return response;
        }

        var httpContext = httpContextAccessor.HttpContext;
        var isAdmin = httpContext.User.IsInRole("Admin");

        response.Result = isAdmin
            ? mapper.Map<AdminViewDTO>(room)
            : mapper.Map<RoomDTO>(room);

        response.StatusCode = HttpStatusCode.OK;
        response.IsSuccess = true;

        return response;
    }


    public async Task<ApiResponse> RoomServiceBook(RoomUpdateDTO updateDto)
    {
        ApiResponse response;
        
        var validationRes = await validatorUpdate.ValidateAsync(updateDto);

        if (!validationRes.IsValid)
        {
            response = new()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add("Necessary field should not be null");
            return response;
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
            return response;
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
            return response;
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

        return response;
    }

    public async Task<ApiResponse> RoomServiceCreate(RoomCreateDTO createDto)
    {
        ApiResponse response;

        var validationRes = await validatorCreate.ValidateAsync(createDto);
        if (!validationRes.IsValid)
        {
            response = new()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Result = null,
                IsSuccess = false,
            };
            response.ErrorMessages.Add("Necessary field should not be null");
            return response;
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
            return response;
        }

        var room = mapper.Map<Models.Room>(createDto);
        await roomRepo.CreateAsync(room);
        await roomRepo.SaveAsync();

        RoomDTO result = mapper.Map<RoomDTO>(room);
        
        response = new()
        {
            StatusCode = HttpStatusCode.Created,
            Result = result,
            IsSuccess = true,
        };

        return response;
    }

    public async Task<ApiResponse> RoomServiceDelete(int id)
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
            return response;
        }
        
        await roomRepo.DeleteAsync(room);
        await roomRepo.SaveAsync();
        
        response = new()
        {
            StatusCode = HttpStatusCode.OK,
            Result = null,
            IsSuccess = true,
        };

        return response;
    }
}