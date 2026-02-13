using Microsoft.AspNetCore.Mvc;
using PathWay_Solution.Data;

namespace PathWay_Solution.Controllers.ApplicationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class CounterController(PathwayDBContext db) : ControllerBase   //done
    {
        [HttpGet]
        public async Task<IActionResult> GetAllCounter()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCounterById(int? id)
        {
            return Ok();
        }
    }
}
