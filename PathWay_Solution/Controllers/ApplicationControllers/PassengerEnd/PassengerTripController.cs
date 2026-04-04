using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;

namespace PathWay_Solution.Controllers.ApplicationControllers.PassengerEnd
{
    [Route("api/passenger/trips")]
    [ApiController]
    // [Authorize(Roles = "Passenger")]
    public class PassengerTripController(PathwayDBContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllTrips()
        {
            var trips = await db.Trip
                .Include(t => t.TripSchedule.Route)
                .Include(t => t.Vehicle)
                .Include(t => t.TripSeat)
                    .ThenInclude(ts => ts.Seat)
                .Where(t => t.DepartureTime >= DateTime.Now) // upcoming trips only
                .Select(t => new TripDto
                {
                    TripId = t.TripId,
                    Direction = t.TripSchedule.Direction,
                    DepartureTime = t.DepartureTime,
                    ArrivalTime = t.ArrivalTime,
                    TotalSeats = t.TripSeat.Count(),
                    AvailableSeats = t.TripSeat.Count(ts => !ts.IsBooked)
                })
                .ToListAsync();

            return Ok(trips);
        }

        // GET: /api/passenger/trips/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripById(int id)
        {
            var trip = await db.Trip
                .Include(t => t.TripSchedule.Route)
                .Include(t => t.Vehicle)
                .Include(t => t.TripSeat)
                    .ThenInclude(ts => ts.Seat)
                .Where(t => t.TripId == id)
                .Select(t => new TripDto
                {
                    TripId = t.TripId,
                    Direction = t.TripSchedule.Direction,
                    DepartureTime = t.DepartureTime,
                    ArrivalTime = t.ArrivalTime,
                    TotalSeats = t.TripSeat.Count(),
                    AvailableSeats = t.TripSeat.Count(ts => !ts.IsBooked),
                    Seats = t.TripSeat.Select(ts => new SeatDto
                    {
                        TripSeatId = ts.TripSeatId,
                        SeatNumber = ts.Seat.SeatNumber,
                        IsBooked = ts.IsBooked,
                        IsLocked = ts.IsLocked,
                        LockedUntil = ts.LockedUntil
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (trip == null)
                return NotFound("Trip not found");

            return Ok(trip);
        }
    
    //[HttpGet("{tripId}/seats")]
    //    public async Task<IActionResult> GetTripSeats(int tripId)
    //    {
    //        var tripSeats = await db.TripSeat
    //            .Include(ts => ts.Seat)
    //            .Where(ts => ts.TripId == tripId)
    //            .Select(ts => new
    //            {
    //                ts.TripSeatId,
    //                ts.Seat.SeatNumber,
    //                ts.Seat.Row,
    //                ts.Seat.Column,
    //                ts.Seat.IsWindow,
    //                ts.Seat.IsAisle,
    //                ts.IsBooked
                   
    //            })
    //            .ToListAsync();

    //        return Ok(tripSeats);
    //    }
    }

    // DTOs for passenger view
    public class TripDto
    {
        public int TripId { get; set; }
        public string Direction { get; set; } = null!;
        public string VehicleName { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public List<SeatDto>? Seats { get; set; }
    }

    public class SeatDto
    {
        public int TripSeatId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public bool IsBooked { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedUntil { get; set; }
    }
}

