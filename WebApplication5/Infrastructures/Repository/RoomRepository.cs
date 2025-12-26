using Microsoft.EntityFrameworkCore;
using WebApplication5.Models;
using WebApplication5.Repository.IRepository;
using WebApplication5.Context;

namespace WebApplication5.Repository;

public class RoomRepository : IRoomRepository
{
    private readonly ContextDb db;

    public RoomRepository(ContextDb db)
    {
        this.db = db;
    }
    
    public async Task SaveAsync()
    {
        await db.SaveChangesAsync();
    }

    public async Task<Room> GetAsync(int id)
    {
       return await db.Rooms.FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public async Task<Room> GetAsync(string roomName)
    {
        return await db.Rooms.FirstOrDefaultAsync(u => u.RoomName == roomName);
    }

    public async Task UpdateAsync(Room room)
    {
        db.Rooms.Update(room);
    }

    public async Task DeleteAsync(Room room)
    {
        db.Rooms.Remove(room);
    }

    public async Task CreateAsync(Room room)
    {
        db.Rooms.Add(room);
    }
}