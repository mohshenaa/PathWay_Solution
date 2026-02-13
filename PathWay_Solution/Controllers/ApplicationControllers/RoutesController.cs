using Microsoft.AspNetCore.Http;
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
    public class RoutesController(PathwayDBContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllRoutes()
        {
            var routes = await db.Routes
                .Include(r => r.FromLocation)
                .Include(r => r.ToLocation)
                .Select(r => new
                {
                    r.RouteId,
                    From = r.FromLocation.Name,
                    To = r.ToLocation.Name,
                    r.DistanceInKm,
                    r.IsActive
                })
                .ToListAsync();

            return Ok(routes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRouteById(int id)
        {
            var route = await db.Routes
                .Include(r => r.TripStops)
                .Include(r => r.Trips)
                .Include(r => r.Counters)
                .Include(r => r.TripSchedules)
                .Include(r => r.FromLocation)
                .Include(r => r.ToLocation)
                .FirstOrDefaultAsync(r => r.RouteId == id);

            if (route == null)
                return NotFound($"Route id {id} not found");

            return Ok(route);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoute(RoutesCreateDto dto)
        {
            if (dto.FromLocationId == dto.ToLocationId)
                return BadRequest("From and To locations cannot be the same");

            var fromExists = await db.Location
                .AnyAsync(l => l.LocationId == dto.FromLocationId);

            var toExists = await db.Location
                .AnyAsync(l => l.LocationId == dto.ToLocationId);

            if (!fromExists || !toExists)
                return BadRequest("Invalid location id");

            var route = new Routes
            {
                FromLocationId = dto.FromLocationId,
                ToLocationId = dto.ToLocationId,
                DistanceInKm = dto.DistanceInKm,
                IsActive = true
            };

            db.Routes.Add(route);
            await db.SaveChangesAsync();

            return Ok("Route created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(int id, RoutesUpdateDto dto)
        {
            var route = await db.Routes.FindAsync(id);

            if (route == null)
                return NotFound("Route not found");

            if (dto.FromLocationId == dto.ToLocationId)
                return BadRequest("From and To locations cannot be same");

            route.FromLocationId = dto.FromLocationId;
            route.ToLocationId = dto.ToLocationId;
            route.DistanceInKm = dto.DistanceInKm;
            route.IsActive = dto.IsActive;

            await db.SaveChangesAsync();

            return Ok("Route updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var route = await db.Routes
                .Include(r => r.Counters)
                .Include(r => r.Trips)
                .FirstOrDefaultAsync(r => r.RouteId == id);

            if (route == null)
                return NotFound("Route not found");

            if (route.Counters!.Any() || route.Trips!.Any())
                return BadRequest("Cannot delete route. Related data exists.");

            db.Routes.Remove(route);
            await db.SaveChangesAsync();

            return Ok("Route deleted successfully");
        }
    }
}
