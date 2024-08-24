using Microsoft.AspNetCore.Identity;

namespace Coursea.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public double Fee { get; set; }
        public string Professional_exp { get; set; } = string.Empty;
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
