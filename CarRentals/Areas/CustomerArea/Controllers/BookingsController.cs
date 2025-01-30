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

        public ActionResult Index()
        {
            int customerId = 0;

            if (Request.Cookies.TryGetValue("AuthCookie", out var cookieValue))
            {
                var userData = JsonSerializer.Deserialize<JsonElement>(cookieValue);

                if (userData.TryGetProperty("Name", out var nameElement) &&
                    userData.TryGetProperty("Status", out var statusElement)
                    && userData.TryGetProperty("Id", out var idElement))
                {
                    var name = nameElement.GetString();
                    var status = statusElement.GetString();
                    var id = idElement.GetInt32();

                    customerId = id;
                }
            }
            return View(_bookingRepository.GetAllByCustomerId(customerId));
        }

        public ActionResult Delete(int id)
        {
            return View(_bookingRepository.Get(id));
        }


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
