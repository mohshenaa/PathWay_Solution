using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PathWay_Solution.Data.Seeder;
using PathWay_Solution.Dto;
using PathWay_Solution.Models.IdentityModels;

namespace PathWay_Solution.Controllers.IdentityControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
     
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

       
        [HttpPost("create-DriverOrStaff")]
        public async Task<IActionResult> CreateDriverOrStaff(DriverAndStaffCreateDto dto)
        {
            if (dto.Role != AppRoles.Driver && dto.Role != AppRoles.CounterStaff)
                return BadRequest("Invalid staff or driver role");

            var tempPassword = $"Temp@{Guid.NewGuid():N}".Substring(0, 10);

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, tempPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, dto.Role);

            return Ok(new
            {
                message = $"{dto.Role} created successfully",
                temporaryPassword = tempPassword // send via email in real app
            });
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("test-admin")]
        public IActionResult TestAdmin()
        {
            return Ok("Admin access granted");
        }

    }
}
