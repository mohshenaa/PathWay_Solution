using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;
using PathWay_Solution.Models.ApplicationModels;

namespace PathWay_Solution.Controllers.ApplicationControllers.AdminEnd
{
    [Route("api/VehiclePayment")]
    [ApiController]
    [Authorize]
    public class VehiclePaymentController(PathwayDBContext db) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreatePayment(CreateVehiclePaymentDto dto)
        {
            var booking = await db.VehicleBookings
                .FirstOrDefaultAsync(vb => vb.VehicleBookingId == dto.VehicleBookingId);

            if (booking == null)
                return NotFound("Booking not found.");

            if (booking.PaymentStatus == PaymentStatus.Paid)
                return BadRequest("Already paid.");

            if (dto.Amount <= 0)
                return BadRequest("Invalid amount.");

            // Prevent wrong payment
            if (dto.Amount != booking.Price)
                return BadRequest("Payment amount mismatch.");

            var payment = new VehiclePayment
            {
                VehicleBookingId = dto.VehicleBookingId,
                Amount = dto.Amount,
                Method = dto.Method,
                Status = PaymentStatus.Paid
            };

            booking.PaymentStatus = PaymentStatus.Paid;

            //Auto confirm after payment
            if (booking.Status == BookingStatus.Pending)
            {
                booking.Status = BookingStatus.Confirmed;
            }

            db.VehiclePayments.Add(payment);
            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Payment successful"
            });
        }
        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetPaymentsByBooking(int bookingId)
        {
            var payments = await db.VehiclePayments
                .Where(p => p.VehicleBookingId == bookingId)
                .ToListAsync();

            return Ok(payments);
        }
    }
}
