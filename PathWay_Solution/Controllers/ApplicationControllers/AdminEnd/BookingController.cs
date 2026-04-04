using Microsoft.AspNetCore.Authorization;
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


    public class BookingController(PathwayDBContext db) : ControllerBase
    {
        // [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await db.Booking
                .Include(b => b.Passenger)
                .Include(b => b.Trip)
                    .ThenInclude(t => t.Vehicle)
                .Include(b => b.BookingSeats)
                    .ThenInclude(ts => ts.TripSeat)
                .ToListAsync();

            return Ok(bookings);
        }
        //[Authorize(Roles = "Passenger,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto dto)
        {
            //Validate passenger
            var passengerExists = await db.Passenger.AnyAsync(p => p.PassengerId == dto.PassengerId);
            if (!passengerExists) return BadRequest("Passenger not found");

            //Validate trip
            var trip = await db.Trip
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(t => t.TripId == dto.TripId);
            if (trip == null) return BadRequest("Trip not found");

            if (trip.DepartureTime <= DateTime.Now)
                return BadRequest("Trip already started, booking not allowed");

            //Create Booking (Pending)
            var booking = new Booking
            {
                PassengerId = dto.PassengerId,
                TripId = dto.TripId,
                BookingSource = dto.BookingSource,
                BookingStatus = BookingStatus.Pending,
                BookingDate = DateTime.Now
            };
            db.Booking.Add(booking); // Do NOT save yet

            //Handle OnDemand trips (Car/Micro)
            if (trip.TripType == TripType.OnDemand)
            {
                var exists = await db.Booking.AnyAsync(b => b.TripId == dto.TripId && b.BookingStatus == BookingStatus.Confirmed);
                if (exists) return BadRequest("Vehicle already booked");

                // booking.TotalAmount = trip.BaseFare; 
            }
            else
            {
                //Scheduled trips (Bus/MiniBus)
                if (dto.TripSeatId == null || !dto.TripSeatId.Any())
                    return BadRequest("At least one seat must be selected");

                var tripSeats = await db.TripSeat
                       .Include(ts => ts.Seat)
                       .Include(ts => ts.Trip)
                           .ThenInclude(t => t.TripSchedule.Route)
                       .Where(ts => dto.TripSeatId.Contains(ts.TripSeatId) && ts.TripId == dto.TripId)
                       .ToListAsync();

                if (tripSeats.Count != dto.TripSeatId.Count)
                    return BadRequest("Some seats are invalid");

                //Check lock and already booked
                foreach (var ts in tripSeats)
                {
                    if (ts.IsBooked)
                        return BadRequest($"Seat {ts.Seat.SeatNumber} already booked");

                    if (ts.IsLocked && ts.LockedUntil <= DateTime.Now)
                    {
                        ts.IsLocked = false;
                        ts.LockedUntil = null;
                    }

                    if (ts.IsLocked && ts.LockedUntil > DateTime.Now)
                        return BadRequest($"Seat {ts.Seat.SeatNumber} temporarily locked by another user");
                }

                //Lock seats (5 minutes)
                foreach (var ts in tripSeats)
                {
                    ts.IsLocked = true;
                    ts.LockedUntil = DateTime.Now.AddMinutes(5);
                }

                //Create BookingSeat entries using navigation property (EF Core handles FK)
                var bookingSeats = tripSeats.Select(ts => new BookingSeat
                {
                    Booking = booking,
                    TripSeatId = ts.TripSeatId,
                    PriceAtBooking = ts.Trip.TripSchedule.Route.PricePerSeat
                }).ToList();

                booking.TotalAmount = bookingSeats.Sum(bs => bs.PriceAtBooking);

                await db.BookingSeat.AddRangeAsync(bookingSeats);
            }

            await db.SaveChangesAsync();

            return Ok(new BookingResponseDto
            {
                BookingId = booking.BookingId,
                PassengerId = booking.PassengerId,
                TripId = booking.TripId,
                TripSeatId = dto.TripSeatId,
                BookingSource = booking.BookingSource,
                Status = booking.BookingStatus,
                TotalAmount = booking.TotalAmount,
                BookingDate = booking.BookingDate
            });
        }

        [HttpGet("trips/{tripId}/seats")]
        public async Task<IActionResult> GetTripSeats(int tripId)
        {
            var tripSeats = await db.TripSeat
                .Include(ts => ts.Seat)
                .Include(ts => ts.Trip)
                    .ThenInclude(t => t.TripSchedule.Route) // include route to get price
                .Where(ts => ts.TripId == tripId)
                .Select(ts => new
                {
                    ts.TripSeatId,
                    ts.Seat.SeatNumber,
                    ts.Seat.Row,
                    ts.Seat.Column,
                    ts.Seat.IsWindow,
                    ts.Seat.IsAisle,
                    ts.IsBooked,
                    Price = ts.Trip.TripSchedule.Route.PricePerSeat // route price
                })
                .ToListAsync();

            return Ok(tripSeats);
        }

        ////cancel booking
        //[HttpPut("{bookingId}/cancel")]
        //public async Task<IActionResult> CancelBooking(int bookingId)
        //{
        //    var booking = await db.Booking
        //          .Include(b => b.BookingSeats)
        //    .ThenInclude(bs => bs.TripSeat)
        //        .FirstOrDefaultAsync(b => b.BookingId == bookingId);

        //    if (booking == null)
        //        return NotFound("Booking not found");

        //    if (booking.BookingStatus == BookingStatus.Cancelled)
        //        return BadRequest("Already cancelled");

        //    booking.BookingStatus = BookingStatus.Cancelled;

        //    //Free seat
        //    foreach (var bs in booking.BookingSeats!)
        //    {
        //        bs.TripSeat.IsBooked = false;
        //    }

        //    await db.SaveChangesAsync();

        //    return Ok("Booking cancelled successfully");
        //}
    }
}