namespace PathWay_Solution.Models
{
    public class TripStop
    {
        public int TripStopId { get; set; }
        public int RouteId { get; set; }
        public int LocationId { get; set; }
        public int StopOrder { get; set; }
        public int BreakDurationMinutes { get; set; }
        public Routes Routes { get; set; } = null!;
        public Location Location { get; set; } = null!;
    }
}