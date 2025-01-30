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

        public BookingsController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public ActionResult Index()
        {
            return View(_bookingRepository.GetAll().OrderBy(b => b.StartDate));
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
