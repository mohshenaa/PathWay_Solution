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
    public class TripScheduleController(PathwayDBContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllTripSchedule()
        {
            var schedules = await db.TripSchedule
             .Select(a => new TripScheduleCreateDto
             {
                 TripScheduleId = a.TripScheduleId,
                 RouteId = a.RouteId,
                 VehicleType = a.VehicleType,
                 StartTime = a.StartTime,
                 FrequencyHours = a.FrequencyHours,
                 Direction = a.Direction,
                 IsActive = a.IsActive
             })
             .ToListAsync();

            return Ok(schedules);
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TripScheduleCreateDto>>> GetAll()
        //{
        //    var schedules = await db.TripSchedule
        //        .Select(ts => new TripScheduleCreateDto
        //        {
        //            TripScheduleId = ts.TripScheduleId,
        //            RouteId = ts.RouteId,
        //            VehicleType = ts.VehicleType,
        //            StartTime = ts.StartTime,
        //            FrequencyHours = ts.FrequencyHours,
        //            Direction = ts.Direction,
        //            IsActive = ts.IsActive
        //        })
        //        .ToListAsync();

        //    return Ok(schedules);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripScheduleById(int id)
        {
            var schedule = await db.TripSchedule
            .Where(a => a.TripScheduleId == id)
            .Select(a => new TripScheduleCreateDto
            {
                TripScheduleId = a.TripScheduleId,
                RouteId = a.RouteId,
                VehicleType = a.VehicleType,
                StartTime = a.StartTime,
                FrequencyHours = a.FrequencyHours,
                Direction = a.Direction,
                IsActive = a.IsActive
            })
            .FirstOrDefaultAsync();

            if (schedule == null) return NotFound();
            return Ok(schedule);
        }


        [HttpPost]
        public async Task<IActionResult> CreateTripSchedule([FromBody] TripScheduleCreateDto dto)
        {

            var schedule = new TripSchedule
            {
                RouteId = dto.RouteId,
                VehicleType = dto.VehicleType,
                StartTime = dto.StartTime,
                FrequencyHours = dto.FrequencyHours,
                Direction = dto.Direction,
                IsActive = dto.IsActive
            };

            db.TripSchedule.Add(schedule);
            await db.SaveChangesAsync();

            dto.TripScheduleId = schedule.TripScheduleId;
            return CreatedAtAction(nameof(GetTripScheduleById), new { id = dto.TripScheduleId }, dto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTripSchedule(int id, [FromBody] TripScheduleCreateDto dto)
        {
            if (id != dto.TripScheduleId) return BadRequest("ID mismatch");

            var schedule = await db.TripSchedule.FindAsync(id);
            if (schedule == null) return NotFound();

            schedule.RouteId = dto.RouteId;
            schedule.VehicleType = dto.VehicleType;
            schedule.StartTime = dto.StartTime;
            schedule.FrequencyHours = dto.FrequencyHours;
            schedule.Direction = dto.Direction;
            schedule.IsActive = dto.IsActive;

            await db.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await db.TripSchedule.FindAsync(id);
            if (schedule == null) return NotFound();

            db.TripSchedule.Remove(schedule);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
