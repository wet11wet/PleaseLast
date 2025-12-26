namespace WebApplication5.Models;

public class Room
{
    public int Id { get; set; }
    
    public string RoomName { get; set; }
    
    public string Class { get; set; }
    
    public double Price { get; set; }
    
    public bool IsAvailable { get; set; }
    
    public string? Description { get; set; }
}