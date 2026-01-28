using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Dto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;


        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
