using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Dto
{
    public class HelperCreateDto
    {
        [Required]
        public int EmployeeId { get; set; }

    }
    public class HelperUpdateDto
    {
        public int HelperId { get; set; }
        public bool IsAvailable { get; set; }
    }
}
