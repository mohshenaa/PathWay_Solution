using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models.ApplicationModels;

namespace PathWay_Solution.Controllers.ApplicationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class CounterController(PathwayDBContext db) : ControllerBase   //done
    {
        [HttpGet]
        public async Task<IActionResult> GetAllCounter()
        {
            //var counter = await db.Counters
            //    .Select(a=> new
            //    {
            //        a.CounterId,
            //        a.CounterName,
            //        a.ContactNumber,
            //        a.Address
            //    })
            //    .ToListAsync();

            var counter = await db.Counters
                .Include(c => c.CounterStaff)
                    .ThenInclude(cs => cs.Employee)
                .Select(c => new
                {
                    c.CounterId,
                    c.RouteId,
                    c.CounterName,
                    c.Address,
                    c.ContactNumber,                  
                    StaffCount = c.CounterStaff.Count
                })
                .ToListAsync();
            return Ok(counter);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCounterById(int? id)
        {
            var counter= await db.Counters
                .Include(a=>a.Route)
                .Include(a=>a.CounterStaff)
                 .ThenInclude(cs => cs.Employee)
                .FirstOrDefaultAsync(a=>a.CounterId==id);

            if(counter==null) return NotFound($"Counter id {id} not found!");
            return Ok(counter);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCounter(CounterCreateDto dto)
        {
            var route = await db.Routes.AnyAsync(r => r.RouteId == dto.RouteId);

            if (!route)
                return BadRequest("Route not found");

            var counter = new Counters
            {
                RouteId = dto.RouteId,
                CounterName = dto.CounterName,
                Address = dto.Address,
                ContactNumber = dto.ContactNumber
            };

            db.Counters.Add(counter);
            await db.SaveChangesAsync();

            return Ok(new { message = "Counter created successfully", counter.CounterId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCounter(int id, CounterUpdateDto dto)
        {
            var counter = await db.Counters.FindAsync(id);

            if (counter == null)
                return NotFound("Counter not found");

            var routeExists = await db.Routes.AnyAsync(r => r.RouteId == dto.RouteId);

            if (!routeExists)
                return BadRequest("Route not found");

            counter.RouteId = dto.RouteId;
            counter.CounterName = dto.CounterName;
            counter.Address = dto.Address;
            counter.ContactNumber = dto.ContactNumber;

            await db.SaveChangesAsync();

            return Ok("Counter updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCounter(int id)
        {
            var counter = await db.Counters
                .Include(c => c.CounterStaff)
                .FirstOrDefaultAsync(c => c.CounterId == id);

            if (counter == null)
                return NotFound("Counter not found");

            if (counter.CounterStaff.Any())
                return BadRequest("Cannot delete counter with assigned staff");

            db.Counters.Remove(counter);
            await db.SaveChangesAsync();

            return Ok("Counter deleted successfully");
        }
    }
}
