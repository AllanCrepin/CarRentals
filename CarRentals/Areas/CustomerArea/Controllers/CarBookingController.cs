using CarRentals.Data.Models;
using CarRentals.Data.Service;
using CarRentals.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CarRentals.Areas.CustomerArea.Controllers
{
    [Area("CustomerArea")]
    [CustomerOnly]
    public class CarBookingController : Controller
    {
        private readonly ICarRepository _carRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly BookingService _bookingService;
        public CarBookingController(ICarRepository carRepository, IBookingRepository bookingRepository, BookingService bookingService) 
        {
            _carRepository = carRepository;
            _bookingRepository = bookingRepository;
            _bookingService = bookingService;
        }

        public ActionResult Index(int carId)
        {
            var car = _carRepository.Get(carId);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        public ActionResult Success(Booking booking)
        {
            return View(booking);
        }

        [HttpGet]
        public IActionResult GetUnavailableDates(int carId)
        {
            var unavailableDates = _bookingRepository.GetAll()
                .Where(b => b.CarId == carId)
                .SelectMany(b => Enumerable.Range(0, (b.EndDate - b.StartDate).Days + 1)
                .Select(offset => b.StartDate.AddDays(offset)))
                .ToList();

            return Json(unavailableDates.Select(d => d.ToString("yyyy-MM-dd")));

            // If a booking ends on 02 februrary, this sets the 02 as unavailable. Which is desired depending on expected
            // behaviour. Should a car returned on 02 february be available again for the same day?
            // If you want a booking ending 02feb to be available on the same day, simply remove the +1 in
            // .SelectMany(b => Enumerable.Range(0, (b.EndDate - b.StartDate).Days + 1)
        }

        [HttpPost]
        public IActionResult CreateBooking(int carId, string bookingDates)
        {
            int customerId = 1;

            if (Request.Cookies.TryGetValue("AuthCookie", out var cookieValue))
            {
                var userData = JsonSerializer.Deserialize<JsonElement>(cookieValue);

                if (userData.TryGetProperty("Name", out var nameElement) &&
                    userData.TryGetProperty("Status", out var statusElement) &&
                    userData.TryGetProperty("Id", out var idElement))
                {
                    customerId = idElement.GetInt32();
                }
            }

            if (!_bookingService.CarExists(carId))
            {
                ModelState.AddModelError("", "Bilen finns inte i systemet.");
                var carDetails = _carRepository.Get(carId);
                return View("Index", carDetails);
            }

            if (!_bookingService.CustomerExists(customerId))
            {
                ModelState.AddModelError("", "Kunden finns inte i systemet.");
                var carDetails = _carRepository.Get(carId);
                return View("Index", carDetails);
            }

            try
            {
                var booking = _bookingService.CreateBooking(bookingDates, customerId, carId);
                return RedirectToAction("Success", booking);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var carDetails = _carRepository.Get(carId);
                return View("Index", carDetails);
            }
        }
    }
}
