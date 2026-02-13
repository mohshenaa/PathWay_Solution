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
    public class CounterStaffController(PathwayDBContext db) : ControllerBase    //done
    {
        [HttpGet]
        public async Task<IActionResult> GetAllCounterStaff()
        {
            var staff = await db.CounterStaff
                .Include(a => a.Employee)
                 .Select(d => new
                 {
                     d.CounterStaffId,
                     d.EmployeeId,
                     d.Employee.FirstName,
                     d.Employee.LastName,
                     d.Employee.PhoneNumber,
                     d.IsAvailable
                 })
                .ToListAsync();
            return Ok(staff);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCounterStaffById(int? id)
        {
            if (id == null) return BadRequest("Id is required!");

            var staff = await db.CounterStaff
                .Include(a => a.Employee)
                .Include(a => a.Counters)
                .FirstOrDefaultAsync();
            if (staff == null) return BadRequest($"CounterStaff id {id} not found!");
            return Ok(staff);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCounterStaff(CounterStaffCreateDto dto)
        {
            var employee = await db.Employee.FirstOrDefaultAsync(a => a.EmployeeId == dto.EmployeeId && a.IsActive);

            if (employee == null)
            {
                return BadRequest("Employee not found");
            }

            var counterExists = await db.Counters
        .AnyAsync(c => c.CounterId == dto.CounterId);

            if (!counterExists)
                return BadRequest("Invalid CounterId");

            var alreadyAssigned = await db.CounterStaff.AnyAsync(a => a.EmployeeId == dto.EmployeeId) ||
                await db.Helper.AnyAsync(a => a.EmployeeId == dto.EmployeeId) ||
                await db.Driver.AnyAsync(a => a.EmployeeId == dto.EmployeeId);


            if (alreadyAssigned)
                return BadRequest("Employee already assigned to another role");

            var staff = new CounterStaff
            {
                EmployeeId = dto.EmployeeId,
                CounterId = dto.CounterId,
                IsAvailable = true
            };

            db.CounterStaff.Add(staff);
            await db.SaveChangesAsync();

            return Ok("Counter staff assigned successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCounterStaff(int id, CounterStaffUpdateDto dto)
        {
            var staff = await db.CounterStaff.FindAsync(id);
            if (staff == null) return BadRequest("CounterStaff not found!");

            staff.IsAvailable = dto.IsAvailable;

            await db.SaveChangesAsync();
            return Ok("CounterStaff updated successfully");
        }

        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> ChangeAvailability(int id, bool isAvailable)
        {
            var staff = await db.CounterStaff.FindAsync(id);

            if (staff == null)
                return NotFound("CounterStaff not found");

            staff.IsAvailable = isAvailable;
            await db.SaveChangesAsync();

            return Ok("CounterStaff availability updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCounterStaff(int id)
        {
            CounterStaff staff = await db.CounterStaff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            db.CounterStaff.Remove(staff);
            await db.SaveChangesAsync();
            return Ok($"CounterStaff id {id} has been deleted");
        }
    }
}