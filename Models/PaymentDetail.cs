namespace Coursea.Models
{
    public class PaymentDetail
    {
        public int CourseId { get; set; }
        public int PaymentId { get; set; }
        public Course Course { get; set; } = null!;
        public Payment Payment { get; set; } = null!;
    }
}
