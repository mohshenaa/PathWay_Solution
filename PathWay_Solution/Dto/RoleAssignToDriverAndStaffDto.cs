using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Dto
{
    public class RoleAssignToDriverAndStaffDto
    {
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public string FirstName { get; set; } = null!;
    
        public string? Email { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;
    }
}
