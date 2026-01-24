using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required,StringLength(maximumLength:50)]
        public string EmployeeName { get; set; } = null!;

        [Required, StringLength(maximumLength: 30)]
        public string PhoneNumber { get; set; } = null!;
        public string ImageUrl { get; set; } = "";

        [Required, StringLength(maximumLength: 20)]
        public string EmployeeRole { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public ICollection<Salary>? Salaries { get; set; }
    }


}
