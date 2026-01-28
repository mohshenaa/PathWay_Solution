using PathWay_Solution.Models.ApplicationModels;
using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class Seat
    {
        [Key]
        public int SeatId { get; set; }
        public int TripId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public bool IsBooked { get; set; } = false;
        public Trip Trip { get; set; } = null!;
        public Booking? Booking { get; set; } // if passengers book any
    }
}