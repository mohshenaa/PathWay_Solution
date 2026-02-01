using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Models;

namespace PathWay_Solution.Controllers.ApplicationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class LocationController(PathwayDBContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllLocations()
        {
            var location = await db.Location
                .Include(a => a.RoutesFrom)
                .Include(a => a.RoutesTo)
                .Include(a => a.TripStops).ToListAsync();
            return Ok(location);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocationById(int? id)
        {
            if (id == null) return BadRequest("Id is required!");

            var location = await db.Location
                .Include(a => a.RoutesFrom)
                .Include(a => a.RoutesTo)
                .Include(a => a.TripStops).FirstOrDefaultAsync(a => a.LocationId == id);

            if (location == null) return BadRequest($"Location Id {id} is not found!");

            return Ok(location);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLocation(Location location)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lastlocation = await db.Location.OrderByDescending(a => a.LocationId).FirstOrDefaultAsync();
            location.LocationId = lastlocation == null ? 1 : lastlocation.LocationId + 1;

            db.Location.Add(location);
            await db.SaveChangesAsync();

            //var createdLocation= await db.Location
            //    .Include(a=>a.RoutesFrom)
            //    .Include(a=>a.RoutesTo)
            //    .Include(a=>a.TripStops)
            //    .FirstOrDefaultAsync();
            //return Ok(createdLocation);

            return Ok(location);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, Location location)
        {
            if (id != location.LocationId) return BadRequest(new { message = "Id doesn't matched!" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.Entry(location).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!locationExits(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }
       
        private bool locationExits(int id)
        {
            return db.Location.Count(a => a.LocationId == id) > 0;
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            Location location = await db.Location.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            db.Location.Remove(location);
            await db.SaveChangesAsync();
            return Ok($"Location id {id} has been deleted");
        }
    }
}
