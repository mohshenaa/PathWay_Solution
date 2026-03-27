using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;
using System.Security.Claims;

namespace PathWay_Solution.Controllers.ApplicationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]

    public class PassengerController(PathwayDBContext db) : ControllerBase
    {
        // GET: api/Passenger
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PassengerReadDto>>> GetAllPassengers()
        {
            var passengers = await db.Passenger
                                           .Include(p => p.AppUser)
                                           .Include(p => p.Bookings)
                                           .Include(p => p.Reviews)
                                           .Select(p => new PassengerReadDto
                                           {
                                               PassengerId = p.PassengerId,
                                          //     UserId = p.UserId,
                                               Gender = p.Gender,
                                               UserName = p.AppUser.UserName,
                                               BookingCount = p.Bookings.Count,
                                               ReviewCount = p.Reviews.Count
                                           })
                                           .ToListAsync();

            return Ok(passengers);
        }

        // GET: api/Passenger/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PassengerReadDto>> GetPassenger(int id)
        {
            var passenger = await db.Passenger
                                          .Include(p => p.AppUser)
                                          .Include(p => p.Bookings)
                                          .Include(p => p.Reviews)
                                          .Where(p => p.PassengerId == id)
                                          .Select(p => new PassengerReadDto
                                          {
                                              PassengerId = p.PassengerId,
                                             // UserId = p.UserId,
                                              Gender = p.Gender,
                                              UserName = p.AppUser.UserName,
                                              BookingCount = p.Bookings.Count,
                                              ReviewCount = p.Reviews.Count
                                          })
                                          .FirstOrDefaultAsync();

            if (passenger == null)
                return NotFound();

            return Ok(passenger);
        }

        //// POST: api/Passenger
        //[HttpPost]
        //public async Task<ActionResult<PassengerReadDto>> CreatePassenger(PassengerCreateDto dto)
        //{
        //    var passenger = new Passenger
        //    {
        //        AppUserId = dto.AppUserId,
        //        Gender = dto.Gender
        //    };

        //    db.Passenger.Add(passenger);
        //    await db.SaveChangesAsync();

        //    var result = new PassengerReadDto
        //    {
        //        PassengerId = passenger.PassengerId,
        //     //   UserId = passenger.UserId,
        //        Gender = passenger.Gender,
        //        UserName = passenger.AppUser.UserName,
        //        BookingCount = 0,
        //        ReviewCount = 0
        //    };

        //    return CreatedAtAction(nameof(GetPassenger), new { id = passenger.PassengerId }, result);
        //}

        //// PUT: api/Passenger/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdatePassenger(int id, PassengerCreateDto dto)
        //{
        //    var passenger = await db.Passenger.FindAsync(id);
        //    if (passenger == null)
        //        return NotFound();

        //    passenger.AppUserId = dto.AppUserId;
        //    passenger.Gender = dto.Gender; 

        //    await db.SaveChangesAsync();

        //    return NoContent();
        //}

        //// DELETE: api/Passenger/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeletePassenger(int id)
        //{
        //    var passenger = await db.Passenger.FindAsync(id);
        //    if (passenger == null)
        //        return NotFound();

        //    db.Passenger.Remove(passenger);
        //    await db.SaveChangesAsync();

        //    return NoContent();
        //}

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<PassengerReadDto>> GetMyProfile()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            if (!Guid.TryParse(userIdString, out var userId))
                return BadRequest("Invalid user id");

            var passenger = await db.Passenger
                .Include(p => p.AppUser)
                .Where(p => p.AppUserId == userId)
                .Select(p => new PassengerReadDto
                {
                    PassengerId = p.PassengerId,
                    AppUserId = p.AppUserId,
                    Gender = p.Gender,
                    UserName = p.AppUser.UserName,
                    Email = p.AppUser.Email,
                    BookingCount = p.Bookings.Count,
                    ReviewCount = p.Reviews.Count
                })
                .FirstOrDefaultAsync();

            if (passenger == null)
                return NotFound("Passenger profile not found");

            return Ok(passenger);
        }
    }
}
