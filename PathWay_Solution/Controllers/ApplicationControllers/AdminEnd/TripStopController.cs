using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;

namespace PathWay_Solution.Controllers.ApplicationControllers.AdminEnd
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
            var trip = await _db.Routes.FindAsync(dto.RouteId);
            if (trip == null) return BadRequest("Route not found");

            var location = await _db.Location.FindAsync(dto.LocationId);
            if (location == null) return BadRequest("Location not found");

            bool exists = await _db.TripStop.AnyAsync(ts =>
                 ts.RouteId == dto.RouteId &&
                 ts.StopOrder == dto.StopOrder);

            if (exists)
                return BadRequest("Stop order already exists for this route");

            var stop = new TripStop
            {
                RouteId = dto.RouteId,
                LocationId = dto.LocationId,
                StopOrder = dto.StopOrder,
                BreakDurationMinutes = dto.BreakDurationMinutes
            };

            _db.TripStop.Add(stop);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByRoute), new { tripId = stop.RouteId }, new
            {
                stop.TripStopId,
                stop.RouteId,
                stop.LocationId,
                LocationName = location.Name,
                stop.StopOrder,
                stop.BreakDurationMinutes
            });
        }

        [HttpGet("route/{routeId}")]
        public async Task<IActionResult> GetByRoute(int routeId)
        {
            var stops = await _db.TripStop
                .Where(ts => ts.RouteId == routeId)
                .Include(ts => ts.Location)
                .OrderBy(ts => ts.StopOrder)
                .Select(ts => new
                {
                    ts.TripStopId,
                    ts.RouteId,
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
                stop.RouteId,
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