using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;

namespace PathWay_Solution.Controllers.ApplicationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]

    public class TripStopController : ControllerBase
    {
        private readonly PathwayDBContext _db;

        public TripStopController(PathwayDBContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Create(TripStopCreateDto dto)
        {
            var trip = await _db.Trip.FindAsync(dto.TripId);
            if (trip == null) return BadRequest("Trip not found");

            var location = await _db.Location.FindAsync(dto.LocationId);
            if (location == null) return BadRequest("Location not found");

            var stop = new TripStop
            {
                TripId = dto.TripId,
                LocationId = dto.LocationId,
                StopOrder = dto.StopOrder,
                BreakDurationMinutes = dto.BreakDurationMinutes
            };

            _db.TripStop.Add(stop);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByTrip), new { tripId = stop.TripId }, new
            {
                stop.TripStopId,
                stop.TripId,
                stop.LocationId,
                LocationName = location.Name,
                stop.StopOrder,
                stop.BreakDurationMinutes
            });
        }

        [HttpGet("trip/{tripId}")]
        public async Task<IActionResult> GetByTrip(int tripId)
        {
            var stops = await _db.TripStop
                .Where(ts => ts.TripId == tripId)
                .Include(ts => ts.Location)
                .OrderBy(ts => ts.StopOrder)
                .Select(ts => new
                {
                    ts.TripStopId,
                    ts.TripId,
                    ts.LocationId,
                    LocationName = ts.Location.Name,
                    ts.StopOrder,
                    ts.BreakDurationMinutes
                })
                .ToListAsync();

            return Ok(stops);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TripStopUpdateDto dto)
        {
            var stop = await _db.TripStop.FindAsync(id);
            if (stop == null) return NotFound();

            var location = await _db.Location.FindAsync(dto.LocationId);
            if (location == null) return BadRequest("Location not found");

            stop.LocationId = dto.LocationId;
            stop.StopOrder = dto.StopOrder;
            stop.BreakDurationMinutes = dto.BreakDurationMinutes;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                stop.TripStopId,
                stop.TripId,
                stop.LocationId,
                LocationName = location.Name,
                stop.StopOrder,
                stop.BreakDurationMinutes
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var stop = await _db.TripStop.FindAsync(id);
            if (stop == null) return NotFound();

            _db.TripStop.Remove(stop);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}