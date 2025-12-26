using FluentValidation;
using WebApplication5.Models.DTO;

namespace WebApplication5.Validations;

public class UserLoginValidation : AbstractValidator<LoginRequestDTO>
{
    public UserLoginValidation()
    {
        RuleFor(u => u.UserName).NotEmpty();
        RuleFor(u => u.Password).NotEmpty();
    }
}