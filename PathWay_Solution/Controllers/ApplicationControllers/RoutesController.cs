using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;

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
                .Include(a => a.TripStops)
                .Include(a => a.Trips)
                .Include(a => a.Counters)
                .Include(a => a.TripSchedules)
                .Include(a => a.FromLocation)
                .Include(a => a.ToLocation)
                .ToListAsync();
            return Ok(routes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoutesById(int? id)
        {
            if(id==null) return BadRequest("Id is required!");


            var routes = await db.Routes
                .Include(a => a.TripStops)
                .Include(a => a.Trips)
                .Include(a => a.Counters)
                .Include(a => a.TripSchedules)
                .Include(a => a.FromLocation)
                .Include(a => a.ToLocation)
                .FirstOrDefaultAsync(a=>a.RouteId==id);
            if (routes==null)
            {
                return NotFound($"Routes id {id} is not found!");
            }

            return Ok(routes);
        }

    }
}
