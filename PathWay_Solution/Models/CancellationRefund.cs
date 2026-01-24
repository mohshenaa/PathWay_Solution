using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PathWay_Solution.Models
{
    public class CancellationRefund
    {
        [Key]
        public int RefundId { get; set; }
        public int BookingId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RefundAmount { get; set; }
        public DateTime RefundDate { get; set; }
        public string Reason { get; set; } = null!;
        public Booking Booking { get; set; } = null!;
    }

}
