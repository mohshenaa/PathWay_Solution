using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PathWay_Solution.Models
{
    public class Salary
    {
        [Key]
        public int SalaryId { get; set; }

        public int EmployeeId { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasicAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bonus { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Deduction { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetAmount { get; set; }

        public DateTime? PaidDate { get; set; }

        public string Status { get; set; } = "Pending";

        public Employee Employee { get; set; } = null!;
    }
}
