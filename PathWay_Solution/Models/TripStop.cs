using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class TripStop
    {
        [Key]
        public int TripStopId { get; set; }
        public int RouteId { get; set; }
       public int TripId { get; set; }
        public int LocationId { get; set; }
        public int StopOrder { get; set; }
        public int BreakDurationMinutes { get; set; }
        public Trip? Trip { get; set; }
        public Routes Routes { get; set; } = null!;
        public Location Location { get; set; } = null!;
    }
}