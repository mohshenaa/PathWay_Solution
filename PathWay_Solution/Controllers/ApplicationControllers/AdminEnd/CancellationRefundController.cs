using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;
using PathWay_Solution.Models.ApplicationModels;

namespace PathWay_Solution.Controllers.ApplicationControllers.AdminEnd
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class CancellationRefundController(PathwayDBContext db) : ControllerBase
    {
        //[Authorize(Roles = "Passenger")]
        [HttpPost("booking/{bookingId}/refund")]
        public async Task<IActionResult> RequestRefund(int bookingId, [FromBody] RefundRequestDto dto)
        {
            var booking = await db.Booking
                .Include(b => b.Trip)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.TripSeat)
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                return NotFound("Booking not found");

            // Prevent duplicate refund
            var alreadyRefunded = await db.CancellationRefund
                .AnyAsync(r => r.BookingId == bookingId);

            if (alreadyRefunded)
                return BadRequest("Refund already processed");

            // Only paid booking can be refunded
            if (booking.Payment == null || booking.Payment.PaymentStatus != PaymentStatus.Paid)
                return BadRequest("Only paid bookings can be refunded");

            // Prevent refund after trip start
            if (booking.Trip.DepartureTime <= DateTime.Now)
                return BadRequest("Trip already started, refund not allowed");

            // Refund Calculation
            var hoursLeft = (booking.Trip.DepartureTime - DateTime.Now).TotalHours;

            decimal refundAmount;

            if (hoursLeft > 24)
                refundAmount = booking.TotalAmount * 0.9m;
            else if (hoursLeft >= 6)
                refundAmount = booking.TotalAmount * 0.5m;
            else
                refundAmount = 0;

            // Create refund record
            var refund = new CancellationRefund
            {
                BookingId = bookingId,
                RefundAmount = refundAmount,
                RefundDate = DateTime.Now,
                Reason = dto.Reason,
                Status = RefundStatus.Processed
            };

            // Cancel booking
            booking.BookingStatus = BookingStatus.Cancelled;

            // Free seats
            foreach (var bs in booking.BookingSeats!)
            {
                bs.TripSeat.IsBooked = false;
                bs.TripSeat.IsLocked = false;
                bs.TripSeat.LockedUntil = null;
            }

            db.CancellationRefund.Add(refund);
            await db.SaveChangesAsync();

            // Return DTO
            return Ok(new RefundResponseDto
            {
                RefundId = refund.RefundId,
                BookingId = refund.BookingId,
                RefundAmount = refund.RefundAmount,
                RefundDate = refund.RefundDate,
                Reason = refund.Reason,
                Status = refund.Status.ToString()
            });
        }
    }
}
