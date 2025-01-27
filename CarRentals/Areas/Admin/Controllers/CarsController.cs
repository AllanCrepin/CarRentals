using CarRentals.Data.Models;
using CarRentals.Data.Service;
using Microsoft.AspNetCore.Mvc;

namespace CarRentals.Areas.Admin.Controllers
{
    [AdminOnly]
    [Area("Admin")]
    public class CarsController : Controller
    {

        private readonly ICarRepository _carRepository;
        private readonly IBookingRepository _bookingRepository;

        public CarsController(ICarRepository carRepository, IBookingRepository bookingRepository)
        {
            this._carRepository = carRepository;
            this._bookingRepository = bookingRepository;
        }


        // GET: CarsController
        public ActionResult Index()
        {
            return View(_carRepository.GetAll());
        }

        public ActionResult List() { return View(_carRepository.GetAll()); }

        // GET: CarsController/Details/5
        public ActionResult Details(int id)
        {
            return View(_carRepository.Get(id));
        }

        // GET: CarsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CarsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Car car)
        {

            car.ImageUrls.RemoveAll(s => string.IsNullOrEmpty(s));

            try
            {
                if (ModelState.IsValid)
                {
                    _carRepository.Add(car);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CarsController/Edit/5
        public ActionResult Edit(int id)
        {

            var car = _carRepository.Get(id);

            if (car == null)
            {
                return NotFound(); // Return a 404 if the car doesn't exist
            }

            return View(car);


        }

        // POST: CarsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Car car)
        {
            // Clean up empty image URLs
            car.ImageUrls = car.ImageUrls?.Where(url => !string.IsNullOrWhiteSpace(url)).ToList();

            if (!ModelState.IsValid)
            {
                // Return the same view with validation errors
                return View(car);
            }

            try
            {
                // Attempt to update the car
                _carRepository.Update(car);

                // Redirect to index upon successful update
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (optional but recommended)
                // _logger.LogError(ex, "Error updating car with ID {Id}", car.Id);

                // Return the same view with the model to display errors
                ModelState.AddModelError("", "An error occurred while updating the car. Please try again.");
                return View(car);
            }
        }

        // GET: CarsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(_carRepository.Get(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Check if the car has active bookings
                var activeBookings = _bookingRepository.GetAll() // Replace with your actual booking repository method
                    .Where(b => b.CarId == id && !b.IsCancelled)
                    .Any();

                if (activeBookings)
                {
                    // Mark the car as unavailable
                    var car = _carRepository.Get(id); // Replace with your actual method to get a car by id
                    if (car != null)
                    {
                        car.IsAvailable = false;
                        _carRepository.Update(car); // Update the car availability
                        ModelState.AddModelError("", "The car has active bookings and has been marked as unavailable.");
                        //return RedirectToAction(nameof(Delete), new { id });
                        return View("Delete", car);
                    }
                }
                else
                {
                    // Delete the car if there are no active bookings
                    _carRepository.Delete(id);
                    return RedirectToAction(nameof(List)); // Redirect to Index only on successful deletion
                }
            }
            catch (Exception ex)
            {
                // Log the error if needed
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again.");
            }

            // If any issues occur, redirect back to the Delete page with errors
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
