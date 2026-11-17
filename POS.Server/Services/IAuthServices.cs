using POS.Shared.DTOs;

namespace POS.Server.Services
{
    public interface IAuthServices
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    }
}
