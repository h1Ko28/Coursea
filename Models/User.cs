using Microsoft.AspNetCore.Identity;

namespace Coursea.Models
{
    public class User : IdentityUser
    {
        public Instructor Instructor { get; set; }
        public Student Student { get; set; }
    }
}
