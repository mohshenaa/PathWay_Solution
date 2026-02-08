using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Data;
using PathWay_Solution.Data.Seeder;
using PathWay_Solution.Dto;
using PathWay_Solution.Models;
using PathWay_Solution.Models.IdentityModels;

namespace PathWay_Solution.Controllers.IdentityControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController(PathwayDBContext db, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) : ControllerBase
    {
        //private readonly UserManager<AppUser> _userManager;

        //public AdminController(UserManager<AppUser> userManager)
        //{
        //    _userManager = userManager;
        //}


        [HttpPost("AssignRole-DriverOrStaff")]
        public async Task<IActionResult> AssignRoleToDriverOrStaff(RoleAssignToDriverAndStaffDto dto)
        {
            if (dto.Role != AppRoles.Driver && dto.Role != AppRoles.CounterStaff)
                return BadRequest("Invalid staff or driver role");

            var employee= await db.Employee.FindAsync(dto.EmployeeId);

            if (employee != null) return NotFound("Id not found!");

            if (employee.HasLogin) return BadRequest("Login already assigned");

            var tempPassword = $"Temp@{Guid.NewGuid():N}".Substring(0, 10);

            var user = new AppUser
            {
                UserName = employee.Email,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, tempPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await userManager.AddToRoleAsync(user, dto.Role);


            employee.HasLogin = true;
            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Login assigned successfully",
                role = dto.Role,
                temporaryPassword = tempPassword // send via email in real app
            });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await userManager.Users.ToListAsync();

            var UserWithRole = new List<Object>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                UserWithRole.Add(new
                {
                    Id = user.Id,
                    
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    IsActive = user.IsActive,
                    LastLogin = user.LastLogin,
                    CreatedOn = user.CreatedOn,
                    ModifiedOn = user.ModifiedOn,
                    Address = user.Address,
                    Roles = roles
                });
            }
            return Ok(UserWithRole);
        }


        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var role = await roleManager.Roles.Select(a => new { a.Id, a.Name, a.CreatedOn, a.Description }).ToListAsync();
            return Ok(role);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("test-admin")]
        public IActionResult TestAdmin()
        {
            return Ok("Admin access granted");
        }

    }
}
