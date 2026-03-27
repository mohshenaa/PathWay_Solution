using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PathWay_Solution.Models.ApplicationModels
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public int PassengerId { get; set; }
        public int TripId { get; set; }
        public int? PaymentId { get; set; }
        public int? CancellationRefundId { get; set; }
      //  public int? TripSeatId { get; set; } // null for car/micro full rent
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public BookingStatus BookingStatus { get; set; }
        public BookingSource BookingSource { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
     //   public string Status { get; set; } = "Confirmed"; // confirmed, cancelled
        public Passenger Passenger { get; set; } = null!;
        public Trip Trip { get; set; } = null!;
       // public TripSeat? TripSeat { get; set; }
        public CancellationRefund? CancellationRefund { get; set; }
        public ICollection<BookingSeat>? BookingSeat { get; set; }

        public ICollection<Payment>? Payments { get; set; }
    }

    public enum BookingStatus
    {
        Confirmed, Cancelled, Pending
    }
    public enum BookingSource
    {
        Online, Counter, Phone
    }

   
}