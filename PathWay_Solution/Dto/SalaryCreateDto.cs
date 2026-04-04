namespace PathWay_Solution.Dto
{
    public class SalaryCreateDto
    {
        public int EmployeeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public decimal BasicAmount { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
    }
    public class UpdateSalaryDto
    {
        public decimal BasicAmount { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
    }
    public class SalaryResponseDto
    {
        public int SalaryId { get; set; }
        public int EmployeeId { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        public decimal BasicAmount { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deduction { get; set; }
        public decimal NetAmount { get; set; }

        public string Status { get; set; } = null!;
        public DateTime? PaidDate { get; set; }
    }
}
