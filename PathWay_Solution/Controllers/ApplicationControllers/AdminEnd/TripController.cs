using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;
using PathWay_Solution.Services;

namespace PathWay_Solution.Controllers.ApplicationControllers.AdminEnd
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;
        private readonly PathwayDBContext _db;

        public TripController(ITripService tripService, PathwayDBContext db)
        {
            _tripService = tripService;
            _db = db;
        }

       
        [HttpPost]
        public async Task<IActionResult> Create(TripCreateDto dto)
        {
            try
            {
                var trip = await _tripService.CreateTripAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = trip.TripId }, trip);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

     
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trips = await _tripService.GetAllTripsAsync();
            return Ok(trips);
        }

      
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trip = await _tripService.GetTripByIdAsync(id);
            if (trip == null) return NotFound();
            return Ok(trip);
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TripUpdateDto dto)
        {
            try
            {
                var updated = await _tripService.UpdateTripAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _tripService.DeleteTripAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        //start trip
        [HttpPut("{id}/start")]
        public async Task<IActionResult> Start(int id)
        {
            var started = await _tripService.StartTripAsync(id);
            if (!started) return BadRequest("Cannot start this trip.");
            return Ok();
        }

        //complete trip
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var completed = await _tripService.CompleteTripAsync(id);
            if (!completed) return BadRequest("Cannot complete this trip.");
            return Ok();
        }

        //cancel trip
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var cancelled = await _tripService.CancelTripAsync(id);
            if (!cancelled) return BadRequest("Cannot cancel this trip.");
            return Ok();
        }

        //// GET seats / availability for a trip
        //[HttpGet("{id}/seats")]
        //public async Task<IActionResult> GetSeats(int id)
        //{
        //    var trip = await _db.Trip
        //        .Include(t => t.TripSeats)
        //        .Include(t => t.Vehicle)
        //        .FirstOrDefaultAsync(t => t.TripId == id);

        //    if (trip == null) return NotFound();

        //    // EXPRESS trips: return full seat layout with booked info
        //    if (trip.TripType == TripType.Scheduled)
        //    {
        //        var seatLayout = trip.TripSeats?
        //            .Select(s => new
        //            {
        //                s.SeatId,
        //                s.SeatNumber,
        //                s.Row,
        //                s.Column,
        //                s.IsWindow,
        //                s.IsAisle,
        //                s.IsBooked
        //            }).ToList();

        //        return Ok(seatLayout);
        //    }

        //    // LOCAL trips: only return available seat count
        //    int availableSeats = trip.Vehicle.Capacity - (trip.Seat?.Count(s => s.IsBooked) ?? 0);
        //    return Ok(new { TotalSeats = trip.Vehicle.Capacity, AvailableSeats = availableSeats });
        //}
    }
}