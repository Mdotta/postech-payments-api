using Microsoft.AspNetCore.Mvc;

namespace Postech.Payments.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Service = "PaymentsAPI",
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        });
    }
}