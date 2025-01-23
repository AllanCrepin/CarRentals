using CarRentals.Data.Service;
using CarRentals.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CarRentals.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICarRepository _carRepository;
        private readonly IBookingRepository _bookingRepository;

        public HomeController(ILogger<HomeController> logger, ICarRepository carRepository, IBookingRepository bookingRepository)
        {
            _logger = logger;
            _carRepository = carRepository;
            _bookingRepository = bookingRepository;

        }

        public IActionResult Index()
        {
            var topBookedCars = _carRepository.GetAll()
            .Select(car => new
            {
                Car = car,
                BookingCount = _bookingRepository.GetAll().Count(b => b.CarId == car.Id && !b.IsCancelled)
            })
            .OrderByDescending(c => c.BookingCount)
            .Take(3)
            .Select(c => c.Car)
            .ToList();

            // Fallback logic if less than 3 cars are found
            if (topBookedCars.Count < 3)
            {
                var fallbackCars = _carRepository.GetAll()
                    .Take(3)
                    .ToList();

                // Merge the two lists, ensuring no duplicates
                topBookedCars = topBookedCars
                    .Concat(fallbackCars)
                    .Distinct()
                    .Take(3)
                    .ToList();
            }

            return View(topBookedCars);
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
