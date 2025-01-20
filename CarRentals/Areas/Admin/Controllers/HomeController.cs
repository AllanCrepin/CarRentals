using CarRentals.Data.Service;
using CarRentals.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



using AdminModel = CarRentals.Data.Models.Admin;

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
            if (Request.Cookies.ContainsKey("AuthCookie"))
            {
                // The cookie exists, you can retrieve its value
                if (Request.Cookies["AuthCookie"] == "AdminLoggedIn")
                {
                    return View("AdminPage");
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
                    // Create a cookie for authentication
                    var authCookieOptions = new CookieOptions
                    {
                        HttpOnly = true,       // Prevent client-side access
                        Secure = true,         // Ensure cookie is sent over HTTPS (set to false if in development)
                        Expires = DateTime.UtcNow.AddHours(3) // Set expiration time
                    };

                    Response.Cookies.Append("AuthCookie", "AdminLoggedIn", authCookieOptions);

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
