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
        // GET: CarBookingController
        public ActionResult Index(int carId)
        {
            var car = _carRepository.Get(carId);

            if (car == null)
            {
                return NotFound();
            }

            return View(car); // Display the car details where the user can confirm the booking
        }

        public ActionResult Success(Booking booking)
        {

            return View(booking);
        }

        [HttpGet]
        public IActionResult GetUnavailableDates(int carId)
        {
            // Example: Fetch unavailable dates from the database
            var unavailableDates = _bookingRepository.GetAll()
                .Where(b => b.CarId == carId)
                .SelectMany(b => Enumerable.Range(0, (b.EndDate - b.StartDate).Days + 1)
                    .Select(offset => b.StartDate.AddDays(offset)))
                .ToList();

            return Json(unavailableDates.Select(d => d.ToString("yyyy-MM-dd")));
        }




        [HttpPost]
        public IActionResult CreateBooking(int carId, string bookingDates)
        {
            // Retrieve customerId (this should come from the session or logged-in user context)
            var customerId = 1;  // Placeholder for customer ID, replace this with actual logic

            if (Request.Cookies.TryGetValue("AuthCookie", out var cookieValue))
            {
                var userData = JsonSerializer.Deserialize<JsonElement>(cookieValue);

                // Access the properties using JsonElement.GetProperty
                if (userData.TryGetProperty("Name", out var nameElement) &&
                    userData.TryGetProperty("Status", out var statusElement) &&
                    userData.TryGetProperty("Id", out var idElement))
                {
                    customerId = idElement.GetInt32();
                }
            }

            // Validate if the car exists
            if (!_bookingService.CarExists(carId))
            {
                ModelState.AddModelError("", "The selected car does not exist.");
                var carDetails = _carRepository.Get(carId); // Fetch car details for re-rendering
                return View("Index", carDetails);
            }

            // Validate if the customer exists
            if (!_bookingService.CustomerExists(customerId))
            {
                ModelState.AddModelError("", "The customer does not exist.");
                var carDetails = _carRepository.Get(carId); // Fetch car details for re-rendering
                return View("Index", carDetails);
            }

            try
            {
                // Create the booking using the provided parameters
                var booking = _bookingService.CreateBooking(bookingDates, customerId, carId);
                // Redirect to a success page or show the booking details
                return RedirectToAction("Success", booking);
            }
            catch (Exception ex)
            {
                // Handle any errors (e.g., car already booked for the selected dates)
                ModelState.AddModelError("", ex.Message);
                var carDetails = _carRepository.Get(carId); // Fetch car details for re-rendering
                return View("Index", carDetails); // Return to the booking form page with errors
            }
        }



    }
}
