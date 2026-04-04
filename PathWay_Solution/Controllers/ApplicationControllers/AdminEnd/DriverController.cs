using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models.ApplicationModels;

namespace PathWay_Solution.Controllers.ApplicationControllers.AdminEnd
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class DriverController(PathwayDBContext db) : ControllerBase   //done
    {
        [HttpGet]
        public async Task<IActionResult> GetAllDriver()
        {
            var driver = await db.Driver
                .Include(a => a.Employee)
                 .Select(d => new
                 {
                     d.DriverId,
                     d.EmployeeId,
                     d.Employee.FirstName,
                     d.Employee.LastName,
                     d.Employee.PhoneNumber,
                     d.LicenseNumber,
                     d.IsAvailable
                 })
                .ToListAsync();
            return Ok(driver);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDriverById(int? id)
        {
            if (id == null) return BadRequest("Id is required!");

            var driver = await db.Driver
                .Include(a => a.Employee)
                .Include(a => a.Trips)
                .FirstOrDefaultAsync();
            if (driver == null) return BadRequest($"Driver id {id} not found!");
            return Ok(driver);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDriver(DriverCreateDto dto)
        {
            var employee = await db.Employee.FirstOrDefaultAsync(a => a.EmployeeId == dto.EmployeeId && a.IsActive);

            if (employee == null)
            {
                return BadRequest("Employee not found");
            }

            var alreadyAssigned = await db.Driver.AnyAsync(a => a.EmployeeId == dto.EmployeeId) ||
                await db.Helper.AnyAsync(a => a.EmployeeId == dto.EmployeeId) ||
                await db.CounterStaff.AnyAsync(a => a.EmployeeId == dto.EmployeeId);


            if (alreadyAssigned)
                return BadRequest("Employee already assigned to another role");

            var driver = new Driver
            {
                EmployeeId = dto.EmployeeId,
                LicenseNumber = dto.LicenseNumber,
                IsAvailable = true
            };

            db.Driver.Add(driver);
            await db.SaveChangesAsync();

            return Ok("Driver assigned successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDriver(int id, DriverUpdateDto dto)
        {
            var driver = await db.Driver.FindAsync(id);
            if (driver == null) return BadRequest("Driver not found!");

            driver.LicenseNumber = dto.LicenseNumber;
            driver.IsAvailable = dto.IsAvailable;

            await db.SaveChangesAsync();
            return Ok("Driver updated successfully");
        }

        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> ChangeAvailability(int id, bool isAvailable)
        {
            var driver = await db.Driver.FindAsync(id);

            if (driver == null)
                return NotFound("Driver not found");

            driver.IsAvailable = isAvailable;
            await db.SaveChangesAsync();

            return Ok("Driver availability updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            Driver driver = await db.Driver.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }
            db.Driver.Remove(driver);
            await db.SaveChangesAsync();
            return Ok($"Driver id {id} has been deleted");
        }

    }
}
