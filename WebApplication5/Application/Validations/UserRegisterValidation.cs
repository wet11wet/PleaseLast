using FluentValidation;
using WebApplication5.Models.DTO;

namespace WebApplication5.Validations;

public class UserRegisterValidation : AbstractValidator<RegistrationRequestDTO>
{
    public UserRegisterValidation()
    {
        RuleFor(u => u.Password).NotEmpty();
        
        RuleFor(u => u.Name).NotEmpty();

        RuleFor(u => u.UserName).NotEmpty();
    }
}