using PathWay_Solution.Models;

namespace PathWay_Solution.Dto
{
    public class CreateVehiclePaymentDto
    {
        public int VehicleBookingId { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod Method { get; set; }
    }
}
