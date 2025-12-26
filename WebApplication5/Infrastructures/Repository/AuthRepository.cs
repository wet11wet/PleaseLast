using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication5.Context;
using WebApplication5.Models;
using WebApplication5.Models.DTO;
using WebApplication5.Repository.IRepository;

namespace WebApplication5.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly ContextDb db;
    private readonly IMapper mapper;
    private readonly IConfiguration configuration;
    private readonly string secretKey;
    
    public AuthRepository(ContextDb db, IMapper mapper, IConfiguration configuration)
    {
        this.configuration = configuration;
        secretKey = this.configuration.GetValue<string>("ApiSettings:SecretJwtKey");
        this.db = db;
        this.mapper = mapper;
    }
    
    public bool IsUserUnique(string username)
    {
        return !db.Users.Any(u => u.UserName == username);
    }
    
    public async Task<LoginResponseDTO> Login(LoginRequestDTO model)
    {
        User user = await db.Users.SingleOrDefaultAsync(u => u.UserName == model.UserName &&
                                                              u.Password == model.Password);
        if (user is null)
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDesctiptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDesctiptor);

        LoginResponseDTO response = new LoginResponseDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            User = mapper.Map<UserDTO>(user)
        };

        return response;
    }

    public async Task<UserDTO> Register(RegistrationRequestDTO model)
    {
        User user = new()
        {
            Name = model.Name,
            Password = model.Password,
            UserName = model.UserName,
            Role = "Casual"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return mapper.Map<UserDTO>(user);
    }
}