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

            // Get the current user ID or some form of user identifier
            /*
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (Request.Cookies.ContainsKey("AuthCookie"))
            {
                // The cookie exists, you can retrieve its value
                if (Request.Cookies["AuthCookie"] == "AdminLoggedIn")
                {
                    return View("AdminPage");
                }
            }

            // If no user is logged in, return null
            if (string.IsNullOrEmpty(userId))
            {
                return View(null);
            }

            // Retrieve user details using the repository
            var user = _userRepository.GetUserById(userId);

            return View(user);
            */
            if (Request.Cookies.TryGetValue("AuthCookie", out var cookieValue))
            {
                var userData = JsonSerializer.Deserialize<JsonElement>(cookieValue);

                // Access the properties using JsonElement.GetProperty
                if (userData.TryGetProperty("Name", out var nameElement) &&
                    userData.TryGetProperty("Status", out var statusElement))
                {
                    var name = nameElement.GetString();
                    var status = statusElement.GetString();

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
