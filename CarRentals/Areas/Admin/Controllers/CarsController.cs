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

        public ActionResult Index()
        {
            return View(_carRepository.GetAll());
        }

        public ActionResult Details(int id)
        {
            return View(_carRepository.Get(id));
        }

        public ActionResult Create()
        {
            return View();
        }


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

        public ActionResult Edit(int id)
        {
            var car = _carRepository.Get(id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Car car)
        {
            car.ImageUrls = car.ImageUrls?.Where(url => !string.IsNullOrWhiteSpace(url)).ToList();

            if (!ModelState.IsValid)
            {
                return View(car);
            }

            try
            {
                _carRepository.Update(car);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ett fel uppstod. Vänligen försök igen.");
                return View(car);
            }
        }


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
                var activeBookings = _bookingRepository.GetAll()
                    .Where(b => b.CarId == id && !b.IsCancelled)
                    .Any();

                if (activeBookings)
                {
                    var car = _carRepository.Get(id);
                    if (car != null)
                    {
                        car.IsAvailable = false;
                        _carRepository.Update(car);
                        ModelState.AddModelError("", "Bilen har pågående bokningar och har markerats som otillgänglig.");
                        return View("Delete", car);
                    }
                }
                else
                {
                    _carRepository.Delete(id);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ett fel uppstod. Vänligen försök igen.");
            }

            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
