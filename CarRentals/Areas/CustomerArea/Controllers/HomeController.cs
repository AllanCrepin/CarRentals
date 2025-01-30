using CarRentals.Data.Models;
using CarRentals.Data.Service;
using CarRentals.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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


        public ActionResult Index()
        {
            if (Request.Cookies.TryGetValue("AuthCookie", out var cookieValue))
            {
                var userData = JsonSerializer.Deserialize<JsonElement>(cookieValue);

                if (userData.TryGetProperty("Status", out var statusElement))
                {
                    if (statusElement.GetString() == "CustomerLoggedIn")
                    {
                        return View("CustomerPage");
                    }
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
                    var authCookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddHours(3)
                    };
                    
                    var userData = new { Name = customer.Name, Status = "CustomerLoggedIn", Id=customer.Id };
                    var userDataJson = JsonSerializer.Serialize(userData);

                    Response.Cookies.Append("AuthCookie", userDataJson, authCookieOptions);

                    if (IsCarBookingOnHold())
                    {
                        var carIdCookie = Request.Cookies["CarOnHold"];
                        if (carIdCookie != null)
                        {
                            int carId = int.Parse(carIdCookie);
                            Response.Cookies.Delete("CarOnHold");
                            return RedirectToAction("Index", "CarBooking", new { area = "CustomerArea", carId = carId });
                        }

                    }

                    return RedirectToAction(nameof(Index));
                    
                }
                ModelState.AddModelError(string.Empty, "Ogilitig mejladress eller lösenord.");
            }

            var viewModel = new LoginAndRegistrationViewModel
            {
                LoginViewModel = loginViewModel,
                RegistrationViewModel = new CustomerRegistrationViewModel()
            };
            return View("Index", viewModel);
        }

        private bool IsCarBookingOnHold()
        {
            var carIdCookie = Request.Cookies["CarOnHold"];
            if (carIdCookie != null)
            {
                return true;
            }
            return false;
        }

        [CustomerOnly]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthCookie");
            return RedirectToAction("Index", "Home");
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

                if (_customerRepository.GetByEmail(newCustomer.Email) != null)
                {
                    ModelState.AddModelError(string.Empty, "Det finns redan ett konto med den här mejladressen.");
                }
                else
                {
                    var finalCustomer = _customerRepository.Add(newCustomer);

                    var authCookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddHours(3)
                    };

                    var userData = new { Name = newCustomer.Name, Status = "CustomerLoggedIn", Id = finalCustomer.Id };
                    var userDataJson = JsonSerializer.Serialize(userData);

                    Response.Cookies.Append("AuthCookie", userDataJson, authCookieOptions);

                    if (IsCarBookingOnHold())
                    {
                        var carIdCookie = Request.Cookies["CarOnHold"];
                        if (carIdCookie != null)
                        {
                            int carId = int.Parse(carIdCookie);

                            Response.Cookies.Delete("CarOnHold");

                            return RedirectToAction("Index", "CarBooking", new { area = "CustomerArea", carId = carId });
                        }

                    }

                    return RedirectToAction(nameof(SuccessfulRegistration));
                }

            }

            var viewModel = new LoginAndRegistrationViewModel
            {
                LoginViewModel = new CustomerLoginViewModel(),
                RegistrationViewModel = registrationViewModel
            };
            return View("Index", viewModel);
        }
    }
}
