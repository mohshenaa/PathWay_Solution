using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PathWay_Solution.Models.IdentityModels;

namespace PathWay_Solution.Controllers.IdentityControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager
        // , ILogger<UserController> logger
        ) : ControllerBase    //modern c#
    {
        //private readonly UserManager<AppUser> _userManager;      //old c#
        //private readonly RoleManager<AppRole> _roleManager;
        //private readonly ILogger<UserController> _logger;

        //public UserController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager, ILogger<UserController> logger)
        //{
        //    _userManager = userManager;
        //    _roleManager = roleManager;
        //    _logger = logger;
        //}

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



    }
}
