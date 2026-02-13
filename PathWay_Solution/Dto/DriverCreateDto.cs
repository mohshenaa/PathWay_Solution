using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Dto
{
    public class DriverCreateDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required, StringLength(100)]
        public string LicenseNumber { get; set; } = null!;

    }
    public class DriverUpdateDto
    {
        public int DriverId { get; set; }
        [Required, StringLength(100)]
        public string LicenseNumber { get; set; } = null!;

        public bool IsAvailable { get; set; }
    }
}
