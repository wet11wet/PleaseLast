using WebApplication5.Models;

namespace WebApplication5.Repository.IRepository;

public interface IRoomRepository
{
    Task SaveAsync();

    Task<Room> GetAsync(int id);
    
    Task<Room> GetAsync(string roomName);

    Task UpdateAsync(Room room);

    Task DeleteAsync(Room room);

    Task CreateAsync(Room room);
}