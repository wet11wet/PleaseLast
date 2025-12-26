using AutoMapper;
using WebApplication5.Models;
using WebApplication5.Models.DTO;

namespace WebApplication5;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<User, UserDTO>().ReverseMap();
        
        CreateMap<Room, RoomDTO>().ReverseMap();

        CreateMap<Room, RoomCreateDTO>().ReverseMap();

        CreateMap<Room, AdminViewDTO>().ReverseMap();
    }
}