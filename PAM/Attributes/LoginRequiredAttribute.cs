using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PAM.Core.Auth;

namespace PAM.Attributes;

public class LoginRequiredAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.IsVerified())
            context.Result = new RedirectToActionResult("login", "account", null);
    }
}
