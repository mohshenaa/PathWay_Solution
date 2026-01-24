namespace PathWay_Solution.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "Cash"; // cash, card or else
        public string PaymentStatus { get; set; } = "Pending"; // pending, paid, failed
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public Booking Booking { get; set; } = null!;
    }
}