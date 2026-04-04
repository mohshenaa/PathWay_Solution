namespace PathWay_Solution.Dto
{
    public class RefundRequestDto
    {
        public string Reason { get; set; } = null!;
    }
    public class RefundResponseDto
    {
        public int RefundId { get; set; }
        public int BookingId { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime RefundDate { get; set; }
        public string Reason { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
