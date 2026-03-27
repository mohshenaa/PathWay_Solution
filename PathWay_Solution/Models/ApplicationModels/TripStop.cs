using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class TripStop
    {
        [Key]
        public int TripStopId { get; set; }

        [Required]
        public int TripId { get; set; }   // Link to specific trip

        [Required]
        public int LocationId { get; set; } // Stop location

        [Required]
        public int StopOrder { get; set; }  // 1 = first stop, 2 = second stop

        public int BreakDurationMinutes { get; set; }  // Duration at stop

        public Trip Trip { get; set; } = null!;
        public Location Location { get; set; } = null!;
    }
}