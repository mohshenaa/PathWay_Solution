using PathWay_Solution.Models.ApplicationModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PathWay_Solution.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int BookingId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public string? TransactionId { get; set; } // from gateway

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public Booking Booking { get; set; } = null!;
    }

    public enum PaymentMethod
    {
        Cash,
        Card,
        BKash,
        Nagad
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Refunded,
        Unpaid
    }
}