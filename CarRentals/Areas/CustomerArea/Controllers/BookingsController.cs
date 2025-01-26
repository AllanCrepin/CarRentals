using CarRentals.Data.Models;
using CarRentals.Data.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CarRentals.Areas.CustomerArea.Controllers
{
    [CustomerOnly]
    [Area("CustomerArea")]
    public class BookingsController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        public BookingsController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        // GET: BookingsController
        public ActionResult Index()
        {
            var customerId = 0;

            if (Request.Cookies.TryGetValue("AuthCookie", out var cookieValue))
            {
                var userData = JsonSerializer.Deserialize<JsonElement>(cookieValue);

                // Access the properties using JsonElement.GetProperty
                if (userData.TryGetProperty("Name", out var nameElement) &&
                    userData.TryGetProperty("Status", out var statusElement)
                    && userData.TryGetProperty("Id", out var idElement))
                {
                    var name = nameElement.GetString();
                    var status = statusElement.GetString();
                    var id = idElement.GetInt32();

                    //Int32.TryParse(id, out int realid);

                    customerId = id;
                }
            }
            return View(_bookingRepository.GetAllByCustomerId(customerId));
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

        // POST: BookingsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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


        // GET: CarsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(_bookingRepository.Get(id));
        }

        // POST: CarsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _bookingRepository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
