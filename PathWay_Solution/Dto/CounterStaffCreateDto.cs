using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Dto
{
    public class CounterStaffCreateDto
    {
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public int CounterId { get; set; }


    }
    public class CounterStaffUpdateDto
    {
        public int CounterStaffId { get; set; }
        public bool IsAvailable { get; set; }
    }
}
