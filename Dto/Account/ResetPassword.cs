using System.ComponentModel.DataAnnotations;

namespace Coursea.Dto.Account
{
    public class ResetPassword
    {
        [Required]
        public string Password { get; set; } = string.Empty;
        [Compare("Password", ErrorMessage = "Password is not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
