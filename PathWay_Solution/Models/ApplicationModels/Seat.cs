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

        public string Row { get; set; } = null!;
        public int Column { get; set; }
        public bool IsWindow { get; set; }
        public bool IsAisle { get; set; }

        public bool IsBooked { get; set; } = false;
        public Trip Trip { get; set; } = null!;
        public Booking? Booking { get; set; } // if passengers book any
    }
}