using PathWay_Solution.Models.ApplicationModels;
using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class Seat
    {
        [Key]
        public int SeatId { get; set; }

        public int VehicleId { get; set; }

        public string SeatNumber { get; set; } = null!;
        public string Row { get; set; } = null!;
        public int Column { get; set; }

        public bool IsWindow { get; set; }
        public bool IsAisle { get; set; }

        public Vehicle Vehicle { get; set; } = null!;
    }

    public class BookingSeat
    {
        public int BookingSeatId { get; set; }

        public int BookingId { get; set; }
        public int TripSeatId { get; set; }
        public decimal PriceAtBooking { get; set; }
        public Booking Booking { get; set; } = null!;
        public TripSeat TripSeat { get; set; } = null!;
    }
    public class TripSeat
    {
        [Key]
        public int TripSeatId { get; set; }

        public int TripId { get; set; }
        public int SeatId { get; set; }

        public bool IsBooked { get; set; } = false;

        public bool IsLocked { get; set; } = false; //seat lock
        public DateTime? LockedUntil { get; set; }

        public Trip Trip { get; set; } = null!;
        public Seat Seat { get; set; } = null!;
    }
}