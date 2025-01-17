using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CarRentals.Data.Service
{
    public class CustomerOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var authCookie = context.HttpContext.Request.Cookies["AuthCookie"];

            if (authCookie != "CustomerLoggedIn")
            {
                // Redirect to login if the cookie is missing or invalid
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
