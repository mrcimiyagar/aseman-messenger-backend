using Microsoft.AspNetCore.Mvc;

namespace HostVersion.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [Route("~/api/home")]
        [HttpGet]
        public ActionResult<string> Get() => "Welcome to Aseman Backend Services :)";
    }
}