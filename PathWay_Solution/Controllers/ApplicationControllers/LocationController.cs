using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;

namespace PathWay_Solution.Controllers.ApplicationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class LocationController(PathwayDBContext db) : ControllerBase   //done
    {
        [HttpGet]
        public async Task<IActionResult> GetAllLocations()
        {
            var location = await db.Location
                //.Include(a => a.RoutesFrom)
                //.Include(a => a.RoutesTo)
                //.Include(a => a.TripStops)
                .Select(a => new
                {
                    a.LocationId,
                    a.Name,
                    RoutesFromCount = a.RoutesFrom.Count,
                    RoutesToCount = a.RoutesTo.Count,
                    TripStopsCount = a.TripStops.Count
                })
                .ToListAsync();
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

         //   var location = await db.Location.FindAsync(id);

            if (location == null) return BadRequest($"Location Id {id} is not found!");

            return Ok(location);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLocation(LocationCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var location = new Location
            {
                Name = dto.LocationName
            };

            db.Location.Add(location);
            await db.SaveChangesAsync();

            var response = new LocationResponseDto
            {
                LocationId = location.LocationId,
                LocationName = location.Name
            };
     
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, LocationResponseDto dto)
        {
            if (id != dto.LocationId) return BadRequest(new { message = "Id doesn't matched!" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

          var location=await db.Location.FindAsync(id);

            if(location == null) return NotFound();

            location.Name = dto.LocationName;
           
            try
            {
              
                await db.SaveChangesAsync();
               
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(409,"Conflict!");
            }
            var response = new LocationResponseDto
            {
                LocationId = location.LocationId,
                LocationName = location.Name
            };

            return Ok(response);
        }
       
        //private bool locationExits(int id)
        //{
        //    return db.Location.Count(a => a.LocationId == id) > 0;
        //}


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            Location location = await db.Location.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            if (location.RoutesFrom!.Any() || location.RoutesTo!.Any()|| location.TripStops!.Any())
                return BadRequest("Cannot delete location because of related data.");

            db.Location.Remove(location);
            await db.SaveChangesAsync();
            return Ok($"Location id {id} has been deleted");
        }
       
    }
}
