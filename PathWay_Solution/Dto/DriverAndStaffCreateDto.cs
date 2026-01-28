using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Dto
{
    public class DriverAndStaffCreateDto
    {
        [Required]
        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string? Email { get; set; } = null!;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;
    }
}
