using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PathWay_Solution.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        public string ExpenseType { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }

        public int? VehicleId { get; set; }
        public int? TripId { get; set; }
        public Vehicle? Vehicle { get; set; }
        public Trip? Trip { get; set; }
    }

}
