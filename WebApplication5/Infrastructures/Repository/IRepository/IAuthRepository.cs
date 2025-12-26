using WebApplication5.Models.DTO;

namespace WebApplication5.Repository.IRepository;

public interface IAuthRepository
{
    bool IsUserUnique(string username);
    
    Task<LoginResponseDTO> Login(LoginRequestDTO model);
    
    Task<UserDTO> Register(RegistrationRequestDTO model);
}