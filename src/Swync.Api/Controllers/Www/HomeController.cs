using Microsoft.AspNetCore.Mvc;

namespace Swync.Api.Controllers.Www
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Get() => View();
    }
}