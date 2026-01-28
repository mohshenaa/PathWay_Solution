using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Models.ApplicationModels
{
    public class Counters
    {
        [Key]
        public int CounterId { get; set; }
        public int RouteId { get; set; }
        public int? CounterStaffId { get; set; }
        public string CounterName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string ContactNumber { get; set; } = null!;

        public ICollection<CounterStaff>? CounterStaff { get; set; } = null!;
        public Routes Routes { get; set; } = null!;
    }
}