using CarRentals.Data.Models;
using CarRentals.Data.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarRentals.Areas.Admin.Controllers
{
    [AdminOnly]
    [Area("Admin")]
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        // GET: CustomersController
        public ActionResult Index()
        {
            return View(_customerRepository.GetAll());
        }

        // GET: CustomersController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CustomersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Name,Email,Password")] Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _customerRepository.Add(customer);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CustomersController/Edit/5
        public ActionResult Edit(int id)
        {

            var car = _customerRepository.Get(id);

            if (car == null)
            {
                return NotFound(); // Return a 404 if the car doesn't exist
            }

            return View(car);


        }

        // POST: CustomersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {

            if (!ModelState.IsValid)
            {
                // Return the same view with validation errors
                return View(customer);
            }

            try
            {
                // Attempt to update the car
                _customerRepository.Update(customer);

                // Redirect to index upon successful update
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (optional but recommended)
                // _logger.LogError(ex, "Error updating car with ID {Id}", car.Id);

                // Return the same view with the model to display errors
                ModelState.AddModelError("", "An error occurred while updating the customer. Please try again.");
                return View(customer);
            }
        }

        // GET: CustomersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(_customerRepository.Get(id));
        }

        // POST: CarsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _customerRepository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
