using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public int PassengerId { get; set; }
        public int TripId { get; set; }
        public int? PaymentId { get; set; }
        public int? CancellationRefundId { get; set; }
        public int? SeatId { get; set; } // null for car/micro full rent
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public string BookingSource { get; set; } = "Online"; // online or counter
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Confirmed"; // confirmed, cancelled
        public Passenger Passenger { get; set; } = null!;
        public Trip Trip { get; set; } = null!;
        public Seat? Seat { get; set; }
        public Payment? Payment { get; set; }
        public CancellationRefund? CancellationRefund { get; set; }

       // public ICollection<Payment>? Payments { get; set; }
    }
}