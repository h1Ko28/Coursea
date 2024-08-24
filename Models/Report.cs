using System.ComponentModel.DataAnnotations.Schema;

namespace Coursea.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Sent_at { get; set; }

        [ForeignKey("Student")]
        public int Sent_by { get; set; }
        public Student Student { get; set; }
        public int CourseId { get; set;}
        public Course Course { get; set;}
    }
}
