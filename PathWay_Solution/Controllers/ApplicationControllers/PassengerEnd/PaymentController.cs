using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;
using PathWay_Solution.Models.ApplicationModels;

namespace PathWay_Solution.Controllers.ApplicationControllers.PassengerEnd
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "Passenger,Admin")]
    public class PaymentController(PathwayDBContext db) : ControllerBase
    {
        // CREATE PAYMENT (Pending)
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto dto)
        {
            var booking = await db.Booking
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.TripSeat)
                .Include(b => b.Trip)
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

        // CONFIRM PAYMENT
        [HttpPut("{paymentId}/confirm")]
        public async Task<IActionResult> ConfirmPayment(int paymentId, [FromBody] PaymentConfirmDto dto)
        {
            var payment = await db.Payment
                .Include(p => p.Booking)
                    .ThenInclude(b => b.BookingSeats)
                        .ThenInclude(bs => bs.TripSeat)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Trip)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
                return NotFound("Payment not found");

            if (payment.PaymentStatus == PaymentStatus.Paid)
                return BadRequest("Already paid");

            var booking = payment.Booking;

            // On-demand trip check
            if (booking.Trip.TripType == TripType.OnDemand)
            {
                var exists = await db.Booking
                    .AnyAsync(b => b.TripId == booking.TripId &&
                                   b.BookingStatus == BookingStatus.Confirmed &&
                                   b.BookingId != booking.BookingId);

                if (exists)
                    return BadRequest("Vehicle already booked");
            }

            if (booking.Trip.DepartureTime <= DateTime.Now)
                return BadRequest("Trip already started, payment not allowed");

            using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                payment.PaymentStatus = PaymentStatus.Paid;
                payment.TransactionId = dto.TransactionId ?? payment.TransactionId;
                //booking.BookingStatus = BookingStatus.Confirmed;
                booking.BookingStatus = BookingStatus.Cancelled;

                // Lock → Book seats
                foreach (var bs in booking.BookingSeats!)
                {
                    bs.TripSeat.IsBooked = true;
                    bs.TripSeat.IsLocked = false;
                    bs.TripSeat.LockedUntil = null;
                }

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new PaymentResponseDto
                {
                    PaymentId = payment.PaymentId,
                    BookingId = payment.BookingId,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentStatus = payment.PaymentStatus,
                    TransactionId = payment.TransactionId,
                    TransactionDate = payment.TransactionDate
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                return BadRequest("Payment failed");
            }
        }

        // FAIL PAYMENT
        [HttpPut("{paymentId}/fail")]
        public async Task<IActionResult> FailPayment(int paymentId)
        {
            var payment = await db.Payment
                .Include(p => p.Booking)
                    .ThenInclude(b => b.BookingSeats)
                        .ThenInclude(bs => bs.TripSeat)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null)
                return NotFound("Payment not found");

            payment.PaymentStatus = PaymentStatus.Failed;

            var booking = payment.Booking;

            // RELEASE LOCKED SEATS
            foreach (var bs in booking.BookingSeats!)
            {
                bs.TripSeat.IsLocked = false;
                bs.TripSeat.LockedUntil = null;
            }

            await db.SaveChangesAsync();

            return Ok(new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                TransactionId = payment.TransactionId,
                TransactionDate = payment.TransactionDate
            });
        }

        // GET ALL PAYMENTS FOR A BOOKING
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