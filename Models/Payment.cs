using System.ComponentModel.DataAnnotations.Schema;

namespace Coursea.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public double Amount { get; set; }
        public DateTime Payment_date { get; set; }
        public string status { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public List<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();
    }
}
