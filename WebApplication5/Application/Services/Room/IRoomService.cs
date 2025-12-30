using WebApplication5.Models;
using WebApplication5.Models.DTO;

namespace WebApplication5.Application.Services.Room;

public interface IRoomService
{
    Task<ApiResponse> RoomServiceGet(int id);
    
    Task<ApiResponse> RoomServiceBook(RoomUpdateDTO updateDto);
    
    Task<ApiResponse> RoomServiceCreate(RoomCreateDTO createDto);
    
    Task<ApiResponse> RoomServiceDelete(int id);
}