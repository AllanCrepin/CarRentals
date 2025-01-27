using CarRentals.Data.Models;
using CarRentals.Data.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CarRentals.Controllers
{
    public class CarsController : Controller
    {

        private readonly ICarRepository _carRepository;
        private readonly BookingService _bookingService;

        public CarsController(ICarRepository carRepository, BookingService bookingService)
        {
            this._carRepository = carRepository;
            this._bookingService = bookingService;
        }

        // GET: CarsController
        public ActionResult Index()
        {
            return View(_carRepository.GetAllAvailable());
        }

        public ActionResult ChooseDate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChooseDate(string bookingDates)
        {
            // Parse the date range (assuming it's in "YYYY-MM-DD to YYYY-MM-DD" format)
            var dates = bookingDates?.Split(" till ");
            if (dates == null || dates.Length != 2)
            {

                ModelState.AddModelError("", "Invalid date range. Please select valid dates.");
                return RedirectToAction("ChooseDate"); // Redirect back to the form page
            }

            if (!DateTime.TryParse(dates[0], out DateTime startDate) || !DateTime.TryParse(dates[1], out DateTime endDate))
            {
                // Handle parsing errors
                ModelState.AddModelError("", "Invalid date format. Please select valid dates.");
                return RedirectToAction("ChooseDate");
            }

            // Redirect to the GetAvailableCars method with parameters
            return RedirectToAction("AvailableCars", new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
        }

        [HttpGet]
        public IActionResult AvailableCars(string startDate, string endDate)
        {
            if (!DateTime.TryParse(startDate, out DateTime parsedStartDate) ||
            !DateTime.TryParse(endDate, out DateTime parsedEndDate))
            {
                ModelState.AddModelError("", "Invalid date range.");
                return RedirectToAction("ChooseDate");
            }

            var availableCars = _bookingService.GetAvailableCars(parsedStartDate, parsedEndDate);

            return View("AvailableCars", availableCars);
        }


        public ActionResult BookCar(int carId)
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

                    if (status == "CustomerLoggedIn")
                    {
                        return RedirectToAction("Index", "CarBooking", new { area = "CustomerArea", carId = carId });
                    }
                }
            }

            // Store the car ID in a cookie
            Response.Cookies.Append("CarOnHold", carId.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30) // Expire time for the cookie
            });

            // Redirect to login/register page
            return RedirectToAction("Index", "Home", new { area = "CustomerArea" });
        }

        // GET: CarsController/Details/5
        public ActionResult Details(int id)
        {
            return View(_carRepository.Get(id));
        }

    }
}
