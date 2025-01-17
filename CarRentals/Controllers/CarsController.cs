using CarRentals.Data.Models;
using CarRentals.Data.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        // GET: CarsController/Details/5
        public ActionResult Details(int id)
        {
            return View(_carRepository.Get(id));
        }

    }
}
