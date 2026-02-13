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
            //validating role
            if (dto.Role != AppRoles.Driver && dto.Role != AppRoles.CounterStaff)
                return BadRequest("Invalid staff or driver role");

            //employee existence
            var employee = await db.Employee.FindAsync(dto.EmployeeId);

            if (employee == null) return NotFound("Employee not found");

            if (employee.HasLogin) return BadRequest("Login already assigned");  

            //validate domain role assignment
            if (dto.Role == AppRoles.Driver)
            {
                var isDriver = await db.Driver.AnyAsync(d => d.EmployeeId == dto.EmployeeId);
                if (!isDriver)
                    return BadRequest("Employee is not assigned as Driver");
            }
            if (dto.Role == AppRoles.CounterStaff)
            {
                var isCounterStaff = await db.CounterStaff.AnyAsync(d => d.EmployeeId == dto.EmployeeId);
                if (!isCounterStaff)
                    return BadRequest("Employee is not assigned as CounterStaff");
            }

            //creating identity user
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

            var roleResult = await userManager.AddToRoleAsync(user, dto.Role);

            if (!roleResult.Succeeded)
                return BadRequest(roleResult.Errors);

            // Link between employee and appuser
            employee.HasLogin = true;
            employee.AppUserId = user.Id;
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

        //only admin       
        [HttpGet]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await userManager.GetUsersInRoleAsync("Admin");

            var result = admins.Select(a => new
            {
                a.Id,
                a.FirstName,
                a.LastName,
                a.Email,
                a.IsActive,
                a.CreatedOn
            });

            return Ok(result);
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
