using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PathWay_Solution.Dto;
using PathWay_Solution.Models.IdentityModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PathWay_Solution.Controllers.IdentityControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public readonly UserManager<AppUser> _userManager;
        public readonly SignInManager<AppUser> _signInManager;
        public readonly RoleManager<AppRole> _roleManager;
        public readonly IConfiguration _configuration;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        RoleManager<AppRole> roleManager, IConfiguration configuration)
        {         
            _userManager = userManager;
            _signInManager= signInManager;
            _roleManager= roleManager;
            _configuration= configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationDto dto)
        {
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                IsActive = true,
                CreatedOn = DateTime.UtcNow

            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            //Default role
            await _userManager.AddToRoleAsync(user, "Passenger");
            return Ok("User register successfully!");
        }


        [HttpPost("login")]
        public async Task<IActionResult> login(LoginDto dto)
        {
            var user= await _userManager.FindByEmailAsync(dto.Email);
            if (user == null|| !await _userManager.CheckPasswordAsync(user,dto.Password))
            {
                return Unauthorized("Invalid Credential!");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
               new Claim(JwtRegisteredClaimNames.Email,user.Email!),
               new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var token = new JwtSecurityToken
                (
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationMinutes"])) , signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            return Ok(new
            {
                token= new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
