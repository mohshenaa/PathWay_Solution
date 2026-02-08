using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PathWay_Solution.Data;

namespace PathWay_Solution.Controllers.ApplicationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController(PathwayDBContext db): ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllEmployee()
        {
            return Ok();
        }
    }
}
