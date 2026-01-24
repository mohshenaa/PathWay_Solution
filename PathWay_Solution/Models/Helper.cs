using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models
{
    public class Helper
    {
        [Key]
        public int HelperId { get; set; }
        public int EmployeeId { get; set; }

        public bool IsAvailable { get; set; } = true;
        public Employee Employee { get; set; } = null!;
        public ICollection<Trip>? Trips { get; set; }
    }


}