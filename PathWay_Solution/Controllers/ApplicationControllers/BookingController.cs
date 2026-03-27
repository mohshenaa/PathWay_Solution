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

    public class BookingController(PathwayDBContext db) : ControllerBase
    {
       
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await db.Booking
                .Include(b => b.Passenger)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Vehicle)
                .Include(b => b.BookingSeat)
                    .ThenInclude(ts => ts.TripSeat)
                .ToListAsync();

            return Ok(bookings);
        }


        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto dto)
        {
            //Validate passenger
            if (!await db.Passenger.AnyAsync(p => p.PassengerId == dto.PassengerId))
                return BadRequest("Passenger not found");

            //Validate trip
            var trip = await db.Trip
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(t => t.TripId == dto.TripId);

            if (trip == null)
                return BadRequest("Trip not found");

            //Create booking in PENDING status
            var booking = new Booking
            {
                PassengerId = dto.PassengerId,
                TripId = dto.TripId,
                BookingSource = dto.BookingSource,
                TotalAmount = dto.TotalAmount,
                BookingStatus = BookingStatus.Pending,
                BookingDate = DateTime.Now
            };

            db.Booking.Add(booking);
            await db.SaveChangesAsync();

            //OnDemand vehicles (Car/Micro)
            if (trip.TripType == TripType.OnDemand)
            {
                var exists = await db.Booking.AnyAsync(b => b.TripId == dto.TripId && b.BookingStatus == BookingStatus.Confirmed);
                if (exists)
                    return BadRequest("Vehicle already booked");
            }
            else
            {
                //Scheduled trips (Bus/MiniBus)
                if (dto.TripSeatId == null || !dto.TripSeatId.Any())
                    return BadRequest("At least one seat must be selected");

                var tripSeats = await db.TripSeat
                    .Where(ts => dto.TripSeatId.Contains(ts.TripSeatId) && ts.TripId == dto.TripId)
                    .ToListAsync();

                if (tripSeats.Count != dto.TripSeatId.Count)
                    return BadRequest("Some seats are invalid");

                //Check lock + already booked
                foreach (var ts in tripSeats)
                {
                    if (ts.IsBooked)
                        return BadRequest($"Seat {ts.Seat.SeatNumber} already booked");

                    // Expired lock → reset
                    if (ts.IsLocked && ts.LockedUntil <= DateTime.Now)
                    {
                        ts.IsLocked = false;
                        ts.LockedUntil = null;
                    }

                    // Still locked → block
                    if (ts.IsLocked && ts.LockedUntil > DateTime.Now)
                    {
                        return BadRequest($"Seat {ts.Seat.SeatNumber} temporarily locked by another user");
                    }
                }

                // Lock seats for this booking (default 5 minutes)
                foreach (var ts in tripSeats)
                {
                    ts.IsLocked = true;
                    ts.LockedUntil = DateTime.Now.AddMinutes(5);
                }

                // 8️⃣ Create BookingSeat entries
                var bookingSeats = tripSeats.Select(ts => new BookingSeat
                {
                    BookingId = booking.BookingId,
                    TripSeatId = ts.TripSeatId
                }).ToList();

                await db.BookingSeat.AddRangeAsync(bookingSeats);
            }

            await db.SaveChangesAsync();

            // Return response
            return Ok(new
            {
                booking.BookingId,
                booking.PassengerId,
                booking.TripId,
                dto.TripSeatId,
                booking.BookingSource,
                booking.BookingStatus,
                booking.TotalAmount,
                booking.BookingDate
            });
        }

        //cancel booking
        [HttpPut("{bookingId}/cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var booking = await db.Booking
                  .Include(b => b.BookingSeat)
            .ThenInclude(bs => bs.TripSeat)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                return NotFound("Booking not found");

            if (booking.BookingStatus == BookingStatus.Cancelled)
                return BadRequest("Already cancelled");

            booking.BookingStatus = BookingStatus.Cancelled;

            //Free seat
            foreach (var bs in booking.BookingSeat!)
            {
                bs.TripSeat.IsBooked = false;
            }

            await db.SaveChangesAsync();

            return Ok("Booking cancelled successfully");
        }
    }
}