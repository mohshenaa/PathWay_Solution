using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PathWay_Solution.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        public ExpenseType ExpenseType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Description { get; set; }
   //     public string? CreatedBy { get; set; }
        public int? VehicleId { get; set; }
        public int? TripId { get; set; }
        public int? SalaryId { get; set; }
        public string? Note { get; set; }
        public Vehicle? Vehicle { get; set; }
        public Trip? Trip { get; set; }
    }
    public enum ExpenseType
    {
        Fuel,
        Maintenance,
        Office,
        Toll,
        Salary,
        Other

    }

}
