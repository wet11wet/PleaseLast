namespace WebApplication5.Models.DTO;

public class AdminViewDTO
{
    public string RoomName { get; set; }
    
    public string Class { get; set; }
    
    public double Price { get; set; }
    
    public bool IsAvailable { get; set; }
    
    public string? Description { get; set; }
}