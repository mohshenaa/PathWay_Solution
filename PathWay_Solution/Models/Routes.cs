using System.Diagnostics.Metrics;

namespace PathWay_Solution.Models
{
    public class Routes
    {
        public int RouteId { get; set; }
        public int FromLocationId { get; set; }
        public int ToLocationId { get; set; }
        public double DistanceInKm { get; set; }
        public bool IsActive { get; set; } = true;     
        public Location FromLocation { get; set; } = null!;
        public Location ToLocation { get; set; } = null!;
        public ICollection<TripStop>? TripStops { get; set; }
        public ICollection<Counters>? Counters { get; set; }
        public ICollection<TripSchedule>? TripSchedules { get; set; }
        public ICollection<Trip>? Trips { get; set; }
    }

}