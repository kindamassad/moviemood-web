using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MovieMood.Services;

namespace MovieMood.Filters;

public class RequireLoginAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Session.IsLoggedIn())
            context.Result = new RedirectToActionResult("Login", "Account", null);
    }
}
