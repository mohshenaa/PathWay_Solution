using PathWay_Solution.Models.ApplicationModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PathWay_Solution.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public int BookingId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Cash"; // cash, card or else
        public string PaymentStatus { get; set; } = "Pending"; // pending, paid, failed
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public Booking Booking { get; set; } = null!;
    }
}