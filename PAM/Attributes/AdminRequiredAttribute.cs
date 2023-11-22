using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PAM.Core;
using PAM.Core.Auth;
using PAM.Domain.Contracts;

namespace PAM.Attributes;

public class AdminRequiredAttribute : ActionFilterAttribute
{
    private readonly IAccountRepository _repository;

    public AdminRequiredAttribute(IAccountRepository repository)
        => _repository = repository;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.TryGetUserId(out var id))
            context.Result = new RedirectToActionResult("login", "account", null);

        var user = _repository.GetUserById(id);
        if (user == null || user.Level < 50)
            context.Result = new RedirectToActionResult("forbidden", "dashboard", null);
    }
}
