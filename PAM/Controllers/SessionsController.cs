using Microsoft.AspNetCore.Mvc;
using PAM.Attributes;
using PAM.Core.Sessions;
using PAM.PageModels;

namespace PAM.Controllers;

[LoginRequired]
[Route("sessions")]
public class SessionsController : Controller
{
    private readonly SessionManager _manager;

    public SessionsController(SessionManager manager)
    {
        _manager = manager;
    }

    [HttpGet("terminal/{resourceId:int}")]
    public IActionResult GetTerminal(int resourceId)
    {
        var model = new SessionPageModel { ResourceId = resourceId };
        return View("Session", model);
    }
    
    [Route("ws")]
    public async Task OpenWs(int resId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await _manager.CreateSession(resId, ws, HttpContext.RequestAborted);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}
