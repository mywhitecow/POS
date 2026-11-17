using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace POS.Shared.DTOs
{
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
