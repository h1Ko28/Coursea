using System.ComponentModel.DataAnnotations;

namespace Coursea.Dto.Account
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string? UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string? Role { get; set; } = null;
        public string? Exp { get; set; }
    }
}
