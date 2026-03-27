using PathWay_Solution.Models;

namespace PathWay_Solution.Dto
{
    public class PaymentCreateDto
    {
        public int BookingId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }

    public class PaymentConfirmDto
    {
        public string TransactionId { get; set; } = null!;
    }

    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public string? TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
