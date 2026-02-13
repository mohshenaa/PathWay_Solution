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
    public class HelperController(PathwayDBContext db) : ControllerBase   //done
    {
        [HttpGet]
        public async Task<IActionResult> GetAllHelper()
        {
            var helper = await db.Helper
                .Include(a => a.Employee)
                 .Select(d => new
                 {
                     d.HelperId,
                     d.EmployeeId,
                     d.Employee.FirstName,
                     d.Employee.LastName,
                     d.Employee.PhoneNumber,
                     d.IsAvailable
                 })
                .ToListAsync();
            return Ok(helper);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHelperById(int? id)
        {
            if (id == null) return BadRequest("Id is required!");

            var helper = await db.Helper
                .Include(a => a.Employee)
                .Include(a => a.Trips)
                .FirstOrDefaultAsync();
            if (helper == null) return BadRequest($"Helper id {id} not found!");
            return Ok(helper);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHelper(HelperCreateDto dto)
        {
            var employee = await db.Employee.FirstOrDefaultAsync(a => a.EmployeeId == dto.EmployeeId && a.IsActive);

            if (employee == null)
            {
                return BadRequest("Employee not found");
            }

            var alreadyAssigned = await db.Helper.AnyAsync(a => a.EmployeeId == dto.EmployeeId) ||
                await db.Driver.AnyAsync(a => a.EmployeeId == dto.EmployeeId) ||
                await db.CounterStaff.AnyAsync(a => a.EmployeeId == dto.EmployeeId);


            if (alreadyAssigned)
                return BadRequest("Employee already assigned to another role");

            var helper = new Helper
            {
                EmployeeId = dto.EmployeeId,
                IsAvailable = true
            };

            db.Helper.Add(helper);
            await db.SaveChangesAsync();

            return Ok("Helper assigned successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHelper(int id, DriverUpdateDto dto)
        {
            var helper = await db.Helper.FindAsync(id);
            if (helper == null) return BadRequest("Helper not found!");

            helper.IsAvailable = dto.IsAvailable;

            await db.SaveChangesAsync();
            return Ok("Helper updated successfully");
        }

        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> ChangeAvailability(int id, bool isAvailable)
        {
            var helper = await db.Helper.FindAsync(id);

            if (helper == null)
                return NotFound("Helper not found");

            helper.IsAvailable = isAvailable;
            await db.SaveChangesAsync();

            return Ok("Helper availability updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHelper(int id)
        {
            Helper helper = await db.Helper.FindAsync(id);
            if (helper == null)
            {
                return NotFound();
            }
            db.Helper.Remove(helper);
            await db.SaveChangesAsync();
            return Ok($"Helper id {id} has been deleted");
        }
    }
}
