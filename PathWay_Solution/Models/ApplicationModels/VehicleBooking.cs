using PathWay_Solution.Models.IdentityModels;

namespace PathWay_Solution.Models.ApplicationModels
{
    public class VehicleBooking
    {
        public int VehicleBookingId { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public Guid AppUserId { get; set; }

        public string PickupLocation { get; set; } = null!;
        public string DropLocation { get; set; } = null!;

        public DateTime PickupTime { get; set; }
        public DateTime DropTime { get; set; }
        public decimal Price { get; set; }
        public AppUser AppUser { get; set; } = null!;

        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public ICollection<VehiclePayment>? VehiclePayments { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
    }
}
