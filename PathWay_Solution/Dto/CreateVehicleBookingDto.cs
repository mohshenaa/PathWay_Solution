using PathWay_Solution.Models.ApplicationModels;

namespace PathWay_Solution.Dto
{
    public class CreateVehicleBookingDto
    {
        public int VehicleId { get; set; }

        public string PickupLocation { get; set; } = null!;
        public string DropLocation { get; set; } = null!;

        public DateTime PickupTime { get; set; }
        public DateTime DropTime { get; set; }
    }

    public class CreateVehicleBookingByAdminDto
    {
        public int VehicleId { get; set; }
        public Guid AppUserId { get; set; }

        public string PickupLocation { get; set; } = null!;
        public string DropLocation { get; set; } = null!;

        public DateTime PickupTime { get; set; }
        public DateTime DropTime { get; set; }

        public decimal Price { get; set; }
    }

    public class VehicleBookingDto
    {
        public int VehicleBookingId { get; set; }
        public Guid AppUserId { get; set; }

        public string VehicleType { get; set; } = null!;
        public string VehicleNumber { get; set; } = null!;

        public string PickupLocation { get; set; } = null!;
        public string DropLocation { get; set; } = null!;

        public DateTime PickupTime { get; set; }
        public DateTime DropTime { get; set; }

        public decimal Price { get; set; }

        public string Status { get; set; } = null!;
    }

    public class UpdateBookingStatusDto
    {
        public BookingStatus Status { get; set; }
    }
}
