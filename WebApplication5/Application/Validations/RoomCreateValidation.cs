using FluentValidation;
using WebApplication5.Models.DTO;

namespace WebApplication5.Validations;

public class RoomCreateValidation : AbstractValidator<RoomCreateDTO>
{
    public RoomCreateValidation()
    {
        RuleFor(u => u.IsAvailable).NotNull();
        RuleFor(u => u.Price).NotEmpty();
        RuleFor(u => u.Class).NotEmpty();
        RuleFor(u => u.RoomName).NotEmpty();
    }
}