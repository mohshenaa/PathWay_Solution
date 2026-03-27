using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;
using PathWay_Solution.Models.ApplicationModels;

namespace PathWay_Solution.Controllers.ApplicationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]

    public class PaymentController(PathwayDBContext db) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto dto)
        {
            var booking = await db.Booking
                .FirstOrDefaultAsync(b => b.BookingId == dto.BookingId);

            if (booking == null)
                return NotFound("Booking not found");

            if (booking.BookingStatus != BookingStatus.Pending)
                return BadRequest("Booking is not in pending state");

            var existing = await db.Payment
    .AnyAsync(p => p.BookingId == dto.BookingId && p.PaymentStatus == PaymentStatus.Paid);

            if (existing)
                return BadRequest("Payment already completed");

            var payment = new Payment
            {
                BookingId = dto.BookingId,
                Amount = booking.TotalAmount,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                TransactionDate = DateTime.Now
            };

            db.Payment.Add(payment);
            await db.SaveChangesAsync();

            return Ok(new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                TransactionDate = payment.TransactionDate
            });
        }

        [HttpPut("{paymentId}/confirm")]
        public async Task<IActionResult> ConfirmPayment(int paymentId, [FromBody] PaymentConfirmDto dto)
        {
            var payment = await db.Payment
                .Include(p => p.Booking)
                    .ThenInclude(b => b.BookingSeat)
                        .ThenInclude(bs => bs.TripSeat)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Trip)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
                return NotFound("Payment not found");

            if (payment.PaymentStatus == PaymentStatus.Paid)
                return BadRequest("Already paid");

            var booking = payment.Booking;

            //OnDemand check
            if (booking.Trip.TripType == TripType.OnDemand)
            {
                var exists = await db.Booking
                    .AnyAsync(b => b.TripId == booking.TripId &&
                                   b.BookingStatus == BookingStatus.Confirmed &&
                                   b.BookingId != booking.BookingId);

                if (exists)
                    return BadRequest("Vehicle already booked");
            }

            using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                payment.PaymentStatus = PaymentStatus.Paid;
                payment.TransactionId = dto.TransactionId ?? payment.TransactionId;
                booking.BookingStatus = BookingStatus.Confirmed;

                foreach (var bs in booking.BookingSeat!)
                {
                    bs.TripSeat.IsBooked = true;
                    bs.TripSeat.IsLocked = false;
                    bs.TripSeat.LockedUntil = null;
                }

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok("Payment successful");
            }
            catch
            {
                await transaction.RollbackAsync();
                return BadRequest("Payment failed");
            }

          
        }

        [HttpPut("{paymentId}/fail")]
        public async Task<IActionResult> FailPayment(int paymentId)
        {
            var payment = await db.Payment.FindAsync(paymentId);


            if (payment == null)
                return NotFound("Payment not found");

            payment.PaymentStatus = PaymentStatus.Failed;

            var booking = await db.Booking
    .Include(b => b.BookingSeat)
        .ThenInclude(bs => bs.TripSeat)
    .FirstOrDefaultAsync(b => b.BookingId == payment.BookingId);

            if (booking == null)
                return NotFound("Booking not found");

            // 🔥 RELEASE LOCK
            foreach (var bs in booking.BookingSeat!)
            {
                bs.TripSeat.IsLocked = false;
                bs.TripSeat.LockedUntil = null;
            }
            await db.SaveChangesAsync();

            return Ok("Payment marked as failed");
        }

        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetPaymentsByBooking(int bookingId)
        {
            var payments = await db.Payment
                .Where(p => p.BookingId == bookingId)
                .Select(p => new PaymentResponseDto
                {
                    PaymentId = p.PaymentId,
                    BookingId = p.BookingId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus,
                    TransactionId = p.TransactionId,
                    TransactionDate = p.TransactionDate
                })
                .ToListAsync();

            return Ok(payments);
        }
    }
}
