namespace PathWay_Solution.Models
{
    public class TripSchedule
    {
        public int TripScheduleId { get; set; }
        public int RouteId { get; set; }
        public string VehicleType { get; set; } = "Bus";
        public TimeSpan StartTime { get; set; }
        public int FrequencyHours { get; set; } = 2;
        public string Direction { get; set; } = null!;  //dhaka to ctg 
        public bool IsActive { get; set; } = true;
        public Routes Routes { get; set; } = null!;
        public ICollection<Trip>? Trips { get; set; }
    }
}