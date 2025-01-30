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
                    var authCookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddHours(3)
                    };

                    var userData = new { Name = dbAdmin.Email, Status = "AdminLoggedIn"};
                    var userDataJson = JsonSerializer.Serialize(userData);

                    Response.Cookies.Append("AuthCookie", userDataJson, authCookieOptions);

                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.ErrorMessage = "Invalid username or password.";

            return View("Index");
        }


        [AdminOnly]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthCookie");
            return RedirectToAction("Index", "Home");
        }
    }
}
