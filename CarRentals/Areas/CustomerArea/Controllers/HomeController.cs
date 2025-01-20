using CarRentals.Data.Models;
using CarRentals.Data.Service;
using CarRentals.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarRentals.Areas.CustomerArea.Controllers
{
    [Area("CustomerArea")]
    public class HomeController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly PasswordService _passwordService;
        public HomeController(ICustomerRepository customerRepository, PasswordService passwordService)
        {
            _customerRepository = customerRepository;
            _passwordService = passwordService;
        }
        // GET: HomeController
        public ActionResult Index()
        {
            if (Request.Cookies.ContainsKey("AuthCookie"))
            {
                // The cookie exists, you can retrieve its value
                if (Request.Cookies["AuthCookie"] == "CustomerLoggedIn")
                {
                    return View("CustomerPage");
                }
            }
            return View();
        }

        [CustomerOnly]
        public ActionResult CustomerPage()
        {
            return View();
        }

        public ActionResult SuccessfulRegistration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Authenticate(CustomerLoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var customer = _customerRepository.GetByEmail(loginViewModel.Email);
                if (customer != null && _passwordService.VerifyPassword(loginViewModel.Password, customer.Password))
                {
                    // Create authentication cookie
                    var authCookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddHours(3)
                    };
                    Response.Cookies.Append("AuthCookie", "CustomerLoggedIn", authCookieOptions);
                    return RedirectToAction(nameof(Index));
                    
                }
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }

            // Return to Index with validation errors
            var viewModel = new LoginAndRegistrationViewModel
            {
                LoginViewModel = loginViewModel,
                RegistrationViewModel = new CustomerRegistrationViewModel() // Keep the registration form intact
            };
            return View("Index", viewModel);
        }

        [CustomerOnly]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthCookie"); // Remove the authentication cookie
            return RedirectToAction("Index", "Home");
        }

        // GET: HomeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(CustomerRegistrationViewModel registrationViewModel)
        {
            if (ModelState.IsValid)
            {
                var newCustomer = new Customer
                {
                    Name = registrationViewModel.Name,
                    Email = registrationViewModel.Email,
                    Password = registrationViewModel.Password
                };

                // If a customer with this email address is already registered
                if (_customerRepository.GetByEmail(newCustomer.Email) != null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already registered");
                }
                else
                {
                    _customerRepository.Add(newCustomer);
                    return RedirectToAction(nameof(SuccessfulRegistration));
                }

            }

            // Return to Index with validation errors
            var viewModel = new LoginAndRegistrationViewModel
            {
                LoginViewModel = new CustomerLoginViewModel(), // Keep the login form intact
                RegistrationViewModel = registrationViewModel
            };
            return View("Index", viewModel);
        }
    }
}
