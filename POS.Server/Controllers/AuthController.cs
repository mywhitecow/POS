using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POS.Server.Services;
using POS.Shared.DTOs;

namespace POS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authService;
        public AuthController(IAuthServices authServices)
        {
            _authService = authServices;
        }
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            if (response == null)
            {
                return BadRequest("Usuario ya existe");
            }
            return Ok(response);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
            { 
                return Unauthorized("Credenciales invalidas"); 
            }
            return Ok(response);
        }
    }
}
