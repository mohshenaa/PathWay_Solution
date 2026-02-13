using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Dto
{
    public class EmployeeCreateDto
    {
        [Required, StringLength(50)]
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        [Required, StringLength(30)]
        public string PhoneNumber { get; set; } = null!;

        [EmailAddress]
        public string? Email { get; set; } 
        public string ImageUrl { get; set; } = "";
    }
    public class EmployeeUpdateDto
    {
        public int EmployeeId { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        [Required, StringLength(30)]
        public string PhoneNumber { get; set; } = null!;

        [EmailAddress]
        public string? Email { get; set; }

        public string ImageUrl { get; set; } = "";
        public bool IsActive { get; set; }
    }
   
}
