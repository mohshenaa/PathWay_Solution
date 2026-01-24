namespace PathWay_Solution.Models
{
    public class CancellationRefund
    {
        public int CancellationRefundId { get; set; }
        public int BookingId { get; set; }

        public decimal RefundAmount { get; set; }
        public DateTime RefundDate { get; set; }
        public string Reason { get; set; } = null!;
        public Booking Booking { get; set; } = null!;
    }

}
