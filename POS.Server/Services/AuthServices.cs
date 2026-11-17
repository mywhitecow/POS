using Microsoft.IdentityModel.Tokens;
using POS.Server.Repositories;
using POS.Shared.DTOs;
using POS.Shared.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace POS.Server.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public AuthServices(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return null;
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }
            return GenerateAuthResponse(user);
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
#pragma warning disable CS8629 // Nullable value type may be null.
            if ((bool)await _userRepository.UserExistsAsync(request.Email)) 
            {
                return null; // Usuario ya existe
            }
#pragma warning restore CS8629 // Nullable value type may be null.
                              // Encriptar Password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var newUser = new User
            {
                Name = request.FullName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = request.Role,
            };
            var createdUser = await _userRepository.CreateUserAsync(newUser);
            // Generamos inmediatamente el token o la logica requerida
            return GenerateAuthResponse(createdUser);
        }
        private AuthResponse GenerateAuthResponse(User user)
        {
            var tokenstring = GenerateToken(user);
            return new AuthResponse
            {
                Token = tokenstring,
                UserId = user.Id,
                Email = user.Email,
                FullName = user.Name,
                Role = user.Role.ToString(),
            };
        }
        private string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecrettKey"]!);
            // Deberiamos agregar una validacion si la clave es null o vacio
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpirationMinutes"]!)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
