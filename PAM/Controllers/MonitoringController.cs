using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PAM.Attributes;
using PAM.Services;

namespace PAM.Controllers;

[TypeFilter(typeof(AdminRequiredAttribute))]
[Route("monitoring")]
public class MonitoringController : Controller
{
    private readonly MonitoringService _service;

    public MonitoringController(MonitoringService service)
        => _service = service;

    public IActionResult Index()
        => RedirectToAction("GetAllLogs");

    [HttpGet("logs")]
    public IActionResult GetAllLogs()
        => RedirectToAction("GetLogsPage", new { page = 1 });

    [HttpGet("logs/{page:int}")]
    public async Task<IActionResult> GetLogsPage([Range(1, int.MaxValue)] int page)
    {
        var model = await _service.GetLogsPage(page, HttpContext.RequestAborted);
        return View("Logs", model);
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions()
    {
        var model = await _service.GetSessionsPage(HttpContext.RequestAborted);
        return View("Sessions", model);
    }
}
