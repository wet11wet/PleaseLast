using FluentValidation;
using WebApplication5.Models.DTO;

namespace WebApplication5.Validations;

public class RoomUpdateValidation : AbstractValidator<RoomUpdateDTO>
{
    public RoomUpdateValidation()
    {
        RuleFor(u => u.IsAvailable).NotNull();
    }
}