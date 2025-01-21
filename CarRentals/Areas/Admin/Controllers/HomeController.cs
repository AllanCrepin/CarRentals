using CarRentals.Data.Service;
using CarRentals.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



using AdminModel = CarRentals.Data.Models.Admin;
using System.Text.Json;

namespace CarRentals.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    public class HomeController : Controller
    {

        private readonly IAdminRepository _adminRepository;
        private readonly PasswordService _passwordService;

        public HomeController(IAdminRepository adminRepository, PasswordService passwordService)
        {
            _adminRepository = adminRepository;
            _passwordService = passwordService;
        }

        public ActionResult Index()
        {
            if (Request.Cookies.TryGetValue("AuthCookie", out var cookieValue))
            {
                var userData = JsonSerializer.Deserialize<JsonElement>(cookieValue);

                // Access the properties using JsonElement.GetProperty
                if (userData.TryGetProperty("Name", out var nameElement) &&
                    userData.TryGetProperty("Status", out var statusElement))
                {
                    var name = nameElement.GetString();
                    var status = statusElement.GetString();

                    if (status == "AdminLoggedIn")
                    {
                        return View("AdminPage");
                    }
                }
            }
            /*
            if (Request.Cookies.ContainsKey("AuthCookie"))
            {
                // The cookie exists, you can retrieve its value
                if (cookieValue.name == "AdminLoggedIn")
                {
                    return View("AdminPage");
                }
            }
            return View();
            */

            return View();
        }

        [AdminOnly]
        public ActionResult AdminPage()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authenticate(AdminModel admin)
        {
            if (ModelState.IsValid)
            {
                AdminModel dbAdmin = _adminRepository.GetByEmail(admin.Email);

                if (admin != null && dbAdmin != null && _passwordService.VerifyPassword(admin.Password, dbAdmin.Password))
                {
                    // Create a cookie for authentication
                    var authCookieOptions = new CookieOptions
                    {
                        HttpOnly = true,       // Prevent client-side access
                        Secure = true,         // Ensure cookie is sent over HTTPS (set to false if in development)
                        Expires = DateTime.UtcNow.AddHours(3) // Set expiration time
                    };

                    // Not at ALL secure, but more security has been specifically forbidden.

                    var userData = new { Name = dbAdmin.Email, Status = "AdminLoggedIn"};
                    var userDataJson = JsonSerializer.Serialize(userData);

                    Response.Cookies.Append("AuthCookie", userDataJson, authCookieOptions);

                    return RedirectToAction(nameof(Index)); // Redirect to admin dashboard
                }
            }

            ViewBag.ErrorMessage = "Invalid username or password.";

            return View("Index");
        }


        [AdminOnly]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthCookie"); // Remove the authentication cookie
            return RedirectToAction("Index", "Home");
        }
    }
}
