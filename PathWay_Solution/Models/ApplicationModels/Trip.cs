using PathWay_Solution.Models.ApplicationModels;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

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
        public TripType TripType { get; set; }
        public TripStatus TripStatus { get; set; }
        public Routes Route { get; set; } = null!;
        public TripSchedule TripSchedule { get; set; } = null!;
        public Vehicle Vehicle { get; set; } = null!;
        public Driver Driver { get; set; } = null!;
        public Helper? Helper { get; set; }
        public ICollection<Seat>? Seat { get; set; }
        public ICollection<TripStop>? TripStop { get; set; }
        public ICollection<ReviewRating>? ReviewRating { get; set; }
        public ICollection<Booking>? Booking { get; set; }
    }

    public enum TripType
    {
        Scheduled, OnDemand
    }
    public enum TripStatus
    {
        OnDemand,
        Scheduled,
        Running,
        Completed,
        Cancelled
    }
}