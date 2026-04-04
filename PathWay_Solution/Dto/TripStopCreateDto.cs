using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Dto
{
    public class TripStopCreateDto
    {
        [Required]
        public int RouteId { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public int StopOrder { get; set; }

        public int BreakDurationMinutes { get; set; } = 20; // default
    }

    public class TripStopUpdateDto
    {
        public int LocationId { get; set; }
        public int StopOrder { get; set; }
        public int BreakDurationMinutes { get; set; }
    }

    public class TripStopResponseDto
    {
        public int TripStopId { get; set; }
        public int RouteId { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public int StopOrder { get; set; }
        public int BreakDurationMinutes { get; set; }
    }
}