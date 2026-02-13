using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;
using PathWay_Solution.Services;

namespace PathWay_Solution.Controllers.ApplicationControllers  //done
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class EmployeeController(PathwayDBContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllEmployee()
        {
            var employee = await db.Employee
                .Select(a => new
                {
                    a.EmployeeId,
                    a.FirstName,
                    a.LastName,
                    a.PhoneNumber,
                    a.Email,
                    a.HasLogin,
                    a.ImageUrl,
                    a.IsActive
                }).ToListAsync();
            return Ok(employee);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int? id)
        {
            if (id == null) return BadRequest("Id is required!");

            var employee = await db.Employee.FindAsync(id);

            if (employee == null) return BadRequest($"Employee not found!");

            return Ok(employee);
        }
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeCreateDto dto)
        {
            var exixts = await db.Employee.AnyAsync(a => a.PhoneNumber == dto.PhoneNumber);

            if (exixts) return BadRequest("Employee already exixts!");

            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                ImageUrl = dto.ImageUrl,
                IsActive = true,
                HasLogin = false

            };
            db.Employee.Add(employee);
            await db.SaveChangesAsync();

            var result = new
            {
                EmplyeeId= employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                PhoneNumber = employee.PhoneNumber,
                Email = employee.Email,
                ImageUrl = employee.ImageUrl,
                IsActive = true,
                HasLogin = false
            };

            // return Ok(new { message = "Employee created successfully!" });
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeUpdateDto dto)
        {
            var employee = await db.Employee.FindAsync(id);

            if (employee == null) return BadRequest($"Employee id {id} not found!");

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.PhoneNumber = dto.PhoneNumber;
            employee.ImageUrl = dto.ImageUrl;
            employee.IsActive = true;

            await db.SaveChangesAsync();

            return Ok("Employee updated successfully!");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            Employee employee = await db.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            db.Employee.Remove(employee);
            await db.SaveChangesAsync();
            return Ok($"Employee id {id} has been deleted");
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, bool isActive)
        {
            var employee = await db.Employee.FindAsync(id);

            if (employee == null)
                return NotFound("Employee not found");

            employee.IsActive = isActive;
            await db.SaveChangesAsync();

            return Ok("Employee status updated");
        }

        [HttpPost("Upload/Image")]
        public async Task<IActionResult> UploadEmployeeImage(
       [FromServices] IImageUpload upload,
       [FromForm] UploadFileDto input,
       CancellationToken C)
        {

            var result = await upload.UploadFile(input.File, "Employee", C);
            return Ok(result);
        }
    }
}
