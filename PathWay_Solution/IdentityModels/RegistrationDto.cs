using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.IdentityModels
{
    public class RegistrationDto
    {
        [Required]
        [Display(Name="First Name")]
        [StringLength(50,ErrorMessage ="FirstName cannot be more then 50 character!")]
        public string FirstName { get; set; } = null!;

     
        [Display(Name="Last Name")]
        [StringLength(50,ErrorMessage ="LastName cannot be more then 50 character!")]
        public string? LastName { get; set; }

        [Required(ErrorMessage ="Email address is required")]
        [EmailAddress(ErrorMessage ="Invalid Email!")]
        public string Email { get; set; } = null!;

        [DataType(DataType.Date)]
        [Display(Name = "Date Of Birth")]
        public DateTime? DateOfBirth {  get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        [Phone(ErrorMessage = "Please enter a valid Phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = default!;
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
