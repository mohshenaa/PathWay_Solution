using PathWay_Solution.Models.ApplicationModels;
using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class Trip
    {
        [Key]
        public int TripId { get; set; }
        public int RouteId { get; set; }
        public int TripScheduleId { get; set; }
      //  public int TripStopId { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public int? HelperId { get; set; }
        public int? ReviewRatingId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string TripType { get; set; } = "Scheduled"; // scheduled, ondemand
        public string Status { get; set; } = "Scheduled"; // scheduled, running, completed, cancelled
        public Routes Routes { get; set; } = null!;
        public TripSchedule TripSchedule { get; set; } = null!;
        public Vehicle Vehicle { get; set; } = null!;
        public Driver Driver { get; set; } = null!;
        public Helper? Helper { get; set; }
        public ICollection<Seat>? Seats { get; set; }
        public ICollection<TripStop>? TripStops { get; set; }
        public ICollection<ReviewRating>? ReviewRatings { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}