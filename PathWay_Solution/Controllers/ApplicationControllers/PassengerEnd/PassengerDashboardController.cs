using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models.ApplicationModels;
using System.Security.Claims;

namespace PathWay_Solution.Controllers.ApplicationControllers.PassengerEnd
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassengerDashboardController(PathwayDBContext db) : ControllerBase
    {

        [HttpGet("dashboard")]
        [Authorize]
        public async Task<ActionResult<PassengerDashboardDto>> GetDashboard()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            if (!Guid.TryParse(userIdString, out var userId))
                return BadRequest("Invalid user id");

            var passenger = await db.Passenger
                .Include(p => p.AppUser)
                .Include(p => p.Bookings)
                    .ThenInclude(b => b.Trip)
                        .ThenInclude(t => t.Vehicle)
                .Include(p => p.Bookings)
                    .ThenInclude(b => b.BookingSeats)
                        .ThenInclude(bs => bs.TripSeat)
                            .ThenInclude(ts => ts.Seat)
                .Include(p => p.Bookings)
                    .ThenInclude(b => b.Payment)
                .FirstOrDefaultAsync(p => p.AppUserId == userId);

            if (passenger == null)
                return NotFound("Passenger not found");

            var now = DateTime.Now;

            var mappedBookings = passenger.Bookings.Select(b => new PassengerBookingDto
            {
                BookingId = b.BookingId,
                BookingStatus = b.BookingStatus,
                TotalAmount = b.TotalAmount,
                BookingDate = b.BookingDate,

                TripStatus = b.Trip?.TripStatus.ToString() ?? "Unknown",
                DepartureTime = b.Trip?.DepartureTime ?? default,
                ArrivalTime = b.Trip?.ArrivalTime ?? default,

                VehicleNumber = b.Trip?.Vehicle?.VehicleNumber ?? "N/A",

                Seats = b.BookingSeats != null
                    ? b.BookingSeats
                        .Where(bs => bs.TripSeat != null && bs.TripSeat.Seat != null)
                        .Select(bs => bs.TripSeat.Seat.SeatNumber)
                        .ToList()
                    : new List<string>(),

                PaymentStatus = b.Payment != null
                    ? b.Payment.PaymentStatus.ToString()
                    : "Not Paid",

                RefundAmount = b.CancellationRefund != null ? b.CancellationRefund.RefundAmount : null
            }).ToList();

            var upcoming = mappedBookings
                .Where(b => b.DepartureTime > now && b.BookingStatus != BookingStatus.Cancelled)
                .ToList();

            var completed = mappedBookings
                .Where(b => b.DepartureTime <= now && b.BookingStatus == BookingStatus.Confirmed)
                .ToList();

            var cancelled = mappedBookings
                .Where(b => b.BookingStatus == BookingStatus.Cancelled)
                .ToList();

            var dashboard = new PassengerDashboardDto
            {
                FirstName = passenger.AppUser.FirstName,
                LastName = passenger.AppUser.LastName,
                Email = passenger.AppUser.Email,
                Gender = passenger.Gender,

                TotalBookings = passenger.Bookings.Count,
                CompletedTrips = completed.Count,
                CancelledTrips = cancelled.Count,
                UpcomingTrips = upcoming.Count,

                Upcoming = upcoming,
                Completed = completed,
                Cancelled = cancelled
            };

            return Ok(dashboard);
        }


        //  [HttpGet("me")]
        //  [Authorize]
        //  public async Task<ActionResult<PassengerProfileDto>> GetMyProfile()
        //  {
        //      var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //      if (string.IsNullOrEmpty(userIdString))
        //          return Unauthorized();

        //      if (!Guid.TryParse(userIdString, out var userId))
        //          return BadRequest("Invalid user id");

        //      var passenger = await db.Passenger
        //.Include(p => p.AppUser)
        //.Include(p => p.Bookings)
        //    .ThenInclude(b => b.Trip)
        //        .ThenInclude(t => t.Vehicle)
        //.Include(p => p.Bookings)
        //    .ThenInclude(b => b.BookingSeats)
        //        .ThenInclude(bs => bs.TripSeat)
        //            .ThenInclude(ts => ts.Seat)
        //.Include(p => p.Bookings)
        //    .ThenInclude(b => b.Payment)
        //.Include(p => p.Reviews)
        //.FirstOrDefaultAsync(p => p.AppUserId == userId);

        //      if (passenger == null)
        //          return NotFound("Passenger profile not found");

        //      var result = new PassengerProfileDto
        //      {
        //          PassengerId = passenger.PassengerId,
        //          AppUserId = passenger.AppUserId,
        //          Gender = passenger.Gender,

        //          UserName = passenger.AppUser.UserName,
        //          Email = passenger.AppUser.Email,

        //          BookingCount = passenger.Bookings.Count,
        //          ReviewCount = passenger.Reviews.Count,

        //          Bookings = passenger.Bookings.Select(b =>
        //          new PassengerBookingDto
        //          {
        //              BookingId = b.BookingId,
        //              BookingStatus = b.BookingStatus, 
        //              TotalAmount = b.TotalAmount, 
        //              BookingDate = b.BookingDate, 
        //              TripStatus = b.Trip.TripStatus.ToString(), 
        //              DepartureTime = b.Trip.DepartureTime, 
        //              ArrivalTime = b.Trip.ArrivalTime, 
        //              VehicleNumber = b.Trip.Vehicle.VehicleNumber, 
        //              Seats = b.BookingSeats.Select(bs => bs.TripSeat.Seat.SeatNumber).ToList(), 
        //              PaymentStatus = b.Payment != null ? 
        //              b.Payment.PaymentStatus.ToString() : "Not Paid" 
        //          }).ToList()
        //      };

        //      return Ok(result);
        //  }
    }
}
