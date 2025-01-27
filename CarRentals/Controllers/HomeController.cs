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
        private readonly CarService _carService;

        public HomeController(ILogger<HomeController> logger, ICarRepository carRepository, IBookingRepository bookingRepository, CarService carService)
        {
            _logger = logger;
            _carRepository = carRepository;
            _bookingRepository = bookingRepository;
            _carService = carService;

        }

        public IActionResult Index()
        {
            var topBookedCars = _carService.GetMostBookedCars(3);

            return View(topBookedCars);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult OmOss()
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
