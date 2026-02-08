using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class DriverController(PathwayDBContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllDriver()
        {
            var driver = await db.Driver
                .Include(a => a.Employee)
                .Include(a => a.Trips)
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

        //[HttpPost]
        //public async Task<IActionResult> CreateDriver(DriverCreateDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var driver = new Driver
        //    {
        //        LicenseNumber = dto.LicenseNumber,

        //    };
        //    db.Driver.Add(driver);
        //    await db.SaveChangesAsync();
        //    var response = new DriverResponseDto
        //    {
        //        DriverId = driver.DriverId,
        //        EmployeeId = driver.EmployeeId,
        //        LicenseNumber = driver.LicenseNumber,
        //    };
        //    return Ok(driver);
        //}
    }
}
