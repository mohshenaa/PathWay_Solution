using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.IdentityModels;

namespace PathWay_Solution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AdminController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUser()
        {
            var users= await _userManager.Users.ToListAsync();

            var UserWithRole = new List<Object>();

            foreach (var user in users)
            {
                var roles= await _userManager.GetRolesAsync(user);
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

                });
            }
            return Ok( UserWithRole);
        }
    }
}
