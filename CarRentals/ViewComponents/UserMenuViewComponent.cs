using CarRentals.Data.Service;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CarRentals.ViewComponents
{
    public class UserMenuViewComponent : ViewComponent
    {

        public UserMenuViewComponent()
        {
        }

        
        public IViewComponentResult Invoke()
        {
            if (Request.Cookies.TryGetValue("AuthCookie", out var cookieValue))
            {
                var userData = JsonSerializer.Deserialize<JsonElement>(cookieValue);

                if (userData.TryGetProperty("Name", out var nameElement) &&
                    userData.TryGetProperty("Status", out var statusElement))
                {
                    var name = nameElement.GetString();
                    var status = statusElement.GetString();

                    if (status == "AdminLoggedIn")
                    {
                        ViewBag.Type = "Admin";
                    }
                    if (status == "CustomerLoggedIn")
                    {
                        ViewBag.Type = "Customer";
                    }

                    if (status == "AdminLoggedIn" || status == "CustomerLoggedIn")
                    {
                        ViewBag.Name = name;
                        ViewBag.Status = status;
                        return View();
                    }
                    
                }
            }

            return View();

        }
        
    }
}
