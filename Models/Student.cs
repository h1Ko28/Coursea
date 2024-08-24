using Microsoft.AspNetCore.Identity;

namespace Coursea.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Student_code { get; set; } = string.Empty;
        public string UserId { get; set; }
        public User User { get; set; }
        public List<Report> Reports { get; set; } = new List<Report>();
    }
}