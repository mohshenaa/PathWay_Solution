using PathWay_Solution.Models;
using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Dto
{
    public class TripCreateDto
    {
        [Required]
        public int RouteId { get; set; }

        [Required]
        public int TripScheduleId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int DriverId { get; set; }

        public int? HelperId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        public bool IsExpress { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        public TripType TripType { get; set; }
    }

    public class TripUpdateDto
    {
        public int RouteId { get; set; }
        public int TripScheduleId { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public int? HelperId { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public TripType TripType { get; set; }
    }

    public class TripResponseDto
    {
        public int TripId { get; set; }

        public int RouteId { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public TripType TripType { get; set; }
        public TripStatus TripStatus { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
