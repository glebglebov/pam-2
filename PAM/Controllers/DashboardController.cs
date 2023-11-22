using Microsoft.AspNetCore.Mvc;
using PAM.Attributes;
using PAM.Core.Auth;
using PAM.Services.Dashboard;

namespace PAM.Controllers;

[LoginRequired]
[Route("dashboard")]
public class DashboardController : Controller
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
        => _service = service;

    [HttpGet]
    public IActionResult Index()
        => View("Index");

    [HttpGet("forbidden")]
    public IActionResult Forbidden()
        => View("Forbidden");

    [TypeFilter(typeof(AdminRequiredAttribute))]
    [HttpGet("users")]
    public async Task<IActionResult> Users()
    {
        var model = await _service.GetAllUsers(HttpContext.RequestAborted);
        return View("UserOverview", model);
    }

    [TypeFilter(typeof(AdminRequiredAttribute))]
    [HttpGet("users/{id:int}")]
    public async Task<IActionResult> UserPage(int id)
    {
        var model = await _service.GetUser(id, HttpContext.RequestAborted);
        return View("User", model);
    }

    [TypeFilter(typeof(AdminRequiredAttribute))]
    [HttpPost("users/add")]
    public async Task<IActionResult> CreateUser(string login, string password)
    {
        await _service.CreateUser(login, password, HttpContext.RequestAborted);
        return RedirectToAction("Users");
    }

    [TypeFilter(typeof(AdminRequiredAttribute))]
    [HttpPost("users/add_permission")]
    public async Task<IActionResult> AddUserPermission(int userId, int resourceId)
    {
        await _service.AddPermission(userId, resourceId, HttpContext.RequestAborted);
        return RedirectToAction("UserPage", new { id = userId });
    }

    [TypeFilter(typeof(AdminRequiredAttribute))]
    [HttpPost("users/revoke_permission")]
    public async Task<IActionResult> RevokeUserPermission(int userId, int resourceId)
    {
        await _service.RevokePermission(userId, resourceId, HttpContext.RequestAborted);
        return RedirectToAction("UserPage", new { id = userId });
    }

    [HttpPost("users/set_2fa")]
    public async Task<IActionResult> Set2Fa(int userId, string isEnabled)
    {
        await _service.Set2FaFlag(userId, isEnabled == "on", HttpContext.RequestAborted);
        return RedirectToAction("UserPage", new { id = userId });
    }

    [HttpGet("resources")]
    public async Task<IActionResult> Resources()
    {
        var userId = HttpContext.GetUserId();
        var model = await _service.GetAllResources(userId, HttpContext.RequestAborted);
        return View("ResourceOverview", model);
    }

    [HttpGet("resources/{resourceId:int}")]
    public async Task<IActionResult> Resource(int resourceId)
    {
        var userId = HttpContext.GetUserId();
        if (!await _service.HasAccess(userId, resourceId, HttpContext.RequestAborted))
            return RedirectToAction("Forbidden");
        
        var model = await _service.GetResource(resourceId, HttpContext.RequestAborted);
        return View("Resource", model);
    }

    [TypeFilter(typeof(AdminRequiredAttribute))]
    [HttpPost("resources/add")]
    public async Task<IActionResult> CreateResource(string name, string address)
    {
        await _service.CreateResource(name, address, HttpContext.RequestAborted);
        return RedirectToAction("Resources");
    }
}
