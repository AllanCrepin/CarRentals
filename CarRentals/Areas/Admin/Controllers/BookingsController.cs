using CarRentals.Data.Models;
using CarRentals.Data.Service;
using CarRentals.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentals.Areas.Admin.Controllers
{
    [AdminOnly]
    [Area("Admin")]
    public class BookingsController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly BookingService _bookingService;
        public BookingsController(IBookingRepository bookingRepository, BookingService bookingService)
        {
            _bookingRepository = bookingRepository;
            _bookingService = bookingService;
        }
        // GET: BookingsController
        public ActionResult Index()
        {
            return View(_bookingRepository.GetAll());
        }

        // GET: BookingsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BookingsController/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookingViewModel model)
        {

            /*
            var booking = new Booking
            {
                CustomerId = model.CustomerId,
                CarId = model.CarId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsCancelled = model.IsCancelled
            };

            if (!_bookingService.CarExists(model.CarId))
            {
                ViewBag.ErrorMessage = "Car not found in the database.";
                return View();
            }
            if (!_bookingService.CustomerExists(model.CustomerId))
            {
                ViewBag.ErrorMessage = "Customer not found in the database.";
                return View();
            }
            */


            /*
            var booking = _bookingService.CreateBooking(model);

            try
            {
                if (ModelState.IsValid)
                {
                    _bookingRepository.Add(booking);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
            */

            if (ModelState.IsValid)
            {
                try
                {
                    var booking = _bookingService.CreateBooking(model);
                    _bookingRepository.Add(booking);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);  // Show the error message to the user
                }
            }
            return View(model);


        }

        // GET: BookingsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BookingsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BookingsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BookingsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
