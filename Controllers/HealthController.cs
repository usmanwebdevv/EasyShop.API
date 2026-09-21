using Microsoft.AspNetCore.Mvc;

namespace EasyShop.API.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "Healthy",
                application = "EasyShop.API",
                message = "API is running successfully."
            });
        }
    }
}