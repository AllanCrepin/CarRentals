using CarRentals.Data.Models;
using CarRentals.Data.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CarRentals.Controllers
{
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


        public ActionResult BookCar(int carId)
        {
            if (Request.Cookies.TryGetValue("AuthCookie", out var cookieValue))
            {
                var userData = JsonSerializer.Deserialize<JsonElement>(cookieValue);

                // Access the properties using JsonElement.GetProperty
                if (userData.TryGetProperty("Name", out var nameElement) &&
                    userData.TryGetProperty("Status", out var statusElement))
                {
                    var name = nameElement.GetString();
                    var status = statusElement.GetString();

                    if (status == "CustomerLoggedIn")
                    {
                        return RedirectToAction("Index", "CarBooking", new { area = "CustomerArea", carId = carId });
                    }
                }
            }

            // Store the car ID in a cookie
            Response.Cookies.Append("CarOnHold", carId.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30) // Expire time for the cookie
            });

            // Redirect to login/register page
            return RedirectToAction("Index", "Home", new { area = "CustomerArea" });
        }

        // GET: CarsController/Details/5
        public ActionResult Details(int id)
        {
            return View(_carRepository.Get(id));
        }

    }
}
