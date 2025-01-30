using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Azure.Core;
using System.Text.Json;

namespace CarRentals.Data.Service
{
    public class CustomerOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Cookies.TryGetValue("AuthCookie", out var cookieValue))
            {
                var userData = JsonSerializer.Deserialize<JsonElement>(cookieValue);

                if (userData.TryGetProperty("Name", out var nameElement) &&
                    userData.TryGetProperty("Status", out var statusElement))
                {
                    var name = nameElement.GetString();
                    var status = statusElement.GetString();

                    if (status != "CustomerLoggedIn")
                    {
                        context.Result = new RedirectToActionResult("Index", "Home", null);
                    }
                }
            }
            else
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
