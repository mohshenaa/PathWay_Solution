using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace PathWay_Solution.Models
{
    public class CounterStaff
    {
        [Key]
        public int CounterStaffId { get; set; }
        public int EmployeeId { get; set; }
        public int CounterId { get; set; }
        public Employee Employee { get; set; } = null!;
        public Counters Counters { get; set; } = null!;
    }

}
