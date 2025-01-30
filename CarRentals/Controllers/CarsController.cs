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
            _carRepository = carRepository;
            _bookingService = bookingService;
        }

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
            var dates = bookingDates?.Split(" till ");

            if (dates == null || dates.Length != 2)
            {
                ModelState.AddModelError("", "Ogiltigt datum. Välj ett giltigt datum.");
                return RedirectToAction("ChooseDate");
            }

            if (!DateTime.TryParse(dates[0], out DateTime startDate) || !DateTime.TryParse(dates[1], out DateTime endDate))
            {
                ModelState.AddModelError("", "Ogiltigt datum. Välj ett giltigt datum.");
                return RedirectToAction("ChooseDate");
            }

            return RedirectToAction("AvailableCars", new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
        }

        [HttpGet]
        public IActionResult AvailableCars(string startDate, string endDate)
        {
            if (!DateTime.TryParse(startDate, out DateTime parsedStartDate) ||
            !DateTime.TryParse(endDate, out DateTime parsedEndDate))
            {
                ModelState.AddModelError("", "Ogiltigt datum.");
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

            Response.Cookies.Append("CarOnHold", carId.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30)
            });

            return RedirectToAction("Index", "Home", new { area = "CustomerArea" });
        }

    }
}
