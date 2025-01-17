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

        public CarsController(ICarRepository carRepository)
        {
            this._carRepository = carRepository;
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

        // POST: CarsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _carRepository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
