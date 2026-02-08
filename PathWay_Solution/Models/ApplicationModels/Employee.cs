using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required,StringLength(maximumLength:50)]
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;

        [Required, StringLength(maximumLength: 30)]
        public string PhoneNumber { get; set; } = null!;

        [StringLength(100)]
        public string? Email { get; set; }
        public string ImageUrl { get; set; } = "";

        public bool HasLogin { get; set; } = false; 

        public string? AppUserId { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Salary>? Salaries { get; set; }
    }


}
