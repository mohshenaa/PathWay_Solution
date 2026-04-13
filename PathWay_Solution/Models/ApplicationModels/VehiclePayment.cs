namespace PathWay_Solution.Models.ApplicationModels
{
    public class VehiclePayment
    {
        public int VehiclePaymentId { get; set; }

        public int VehicleBookingId { get; set; }
        public VehicleBooking VehicleBooking { get; set; } = null!;

        public decimal Amount { get; set; }

        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
