using Microsoft.AspNetCore.Mvc;
using PAM.Core.Auth;
using PAM.Services.Account;

namespace PAM.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly IAccountService _service;

    public AccountController(IAccountService service)
        => _service = service;

    public IActionResult Index()
        => RedirectToAction("Login");

    [HttpGet("login")]
    public IActionResult Login()
    {
        if (HttpContext.IsVerified())
            return RedirectToAction("Index", "Dashboard");

        return View("Login");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string login, string password)
    {
        if (HttpContext.IsVerified())
            return RedirectToAction("Index", "Dashboard");

        if (!await _service.TryLogin(login, password, HttpContext.RequestAborted))
            return RedirectToAction("Login");

        return RedirectToAction("VerifyToken");
    }

    [HttpGet("verify_token")]
    public IActionResult VerifyToken()
    {
        if (HttpContext.IsVerified())
            return RedirectToAction("Index", "Dashboard");

        var userId = HttpContext.GetUserId();
        if (userId < 0)
            return RedirectToAction("Login");

        return View("VerifyToken");
    }

    [HttpPost("verify_token")]
    public async Task<IActionResult> VerifyToken(string token)
    {
        if (HttpContext.IsVerified())
            return RedirectToAction("Index", "Dashboard");

        var userId = HttpContext.GetUserId();
        if (userId < 0)
            return RedirectToAction("Login");

        if (!await _service.TryVerifyToken(userId, token, HttpContext.RequestAborted))
            return RedirectToAction("VerifyToken");

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        if (!HttpContext.IsVerified())
            return RedirectToAction("Login");

        HttpContext.Session.Clear();
        HttpContext.ResetUserInfo();

        return RedirectToAction("Login");
    }
}
