using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PathWay_Solution.Data;

namespace PathWay_Solution.Controllers.ApplicationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class RoutesController(PathwayDBContext db) : ControllerBase
    {
    }
}
