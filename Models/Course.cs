using System.ComponentModel.DataAnnotations.Schema;

namespace Coursea.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; }
        public string Status { get; set; } = "Pending";
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public double Fee { get; set; }
        public int Payment_detail_id { get; set; }
        public int Version { get; set; } = 1;
        public string Code_course { get; set; }
        public List<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();
    }
}
