using Microsoft.AspNetCore.Mvc;
using PRUEBA.Server.Services;

namespace PRUEBA.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TwilioController : ControllerBase
{
    [HttpGet("token")]
    public IActionResult GetToken([FromServices] TwilioService twilioService)
        => new JsonResult(twilioService.GetTwilioJwt(User.Identity?.Name));

    [HttpGet("rooms")]
    public async Task<IActionResult> GetRooms([FromServices] TwilioService twilioService)
        => new JsonResult(await twilioService.GetAllRoomsAsync());
}
