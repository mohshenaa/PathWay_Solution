using PathWay_Solution.Models;

namespace PathWay_Solution.Dto
{
    public class ExpenseCreateDto
    {
        public ExpenseType ExpenseType { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Description { get; set; }

        public int? VehicleId { get; set; }
        public int? TripId { get; set; }
    }

    public class ExpenseResponseDto
    {
        public int ExpenseId { get; set; }
        public ExpenseType ExpenseType { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Description { get; set; }

        public int? VehicleId { get; set; }
        public string? VehicleName { get; set; }

        public int? TripId { get; set; }
    }

    public class UpdateExpenseDto
    {
        public ExpenseType ExpenseType { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Description { get; set; }

        public int? VehicleId { get; set; }
        public int? TripId { get; set; }
    }
}
