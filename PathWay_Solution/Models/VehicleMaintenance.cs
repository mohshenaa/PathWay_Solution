using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class VehicleMaintenance
    {
        [Key]
        public int VehicleMaintenanceId { get; set; }
        public int VehicleId { get; set; }

        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Cost { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
    }
}