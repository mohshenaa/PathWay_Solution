using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models.ApplicationModels
{
    public class Driver
    {
        [Key]
        public int DriverId { get; set; }
        public int EmployeeId { get; set; }

        [Required, StringLength(maximumLength: 100)]
        public string LicenseNumber { get; set; } = null!;
        public bool IsAvailable { get; set; } = true;
        public Employee Employee { get; set; } = null!;
        public ICollection<Trip>? Trips { get; set; }
    }

}
