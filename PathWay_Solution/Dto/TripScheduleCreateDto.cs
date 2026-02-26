namespace PathWay_Solution.Dto
{
    public class TripScheduleCreateDto
    {
        public int TripScheduleId { get; set; }
        public int RouteId { get; set; }
        public string VehicleType { get; set; } = "Bus";
        public TimeSpan StartTime { get; set; }
        public int FrequencyHours { get; set; } = 2;
        public string Direction { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}
