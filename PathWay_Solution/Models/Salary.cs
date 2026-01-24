using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PathWay_Solution.Models
{
    public class Salary
    {
        public int SalaryId { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime SalaryMonth { get; set; }
        public DateTime PaidDate { get; set; }
        public Employee Employee { get; set; } = null!;
    }
}
