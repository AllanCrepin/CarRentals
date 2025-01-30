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

        public ActionResult Index()
        {
            return View(_customerRepository.GetAll());
        }

        public ActionResult Create()
        {
            return View();
        }

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

        public ActionResult Edit(int id)
        {
            var car = _customerRepository.Get(id);

            if (car == null)
            {
                return NotFound();
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
                return View(customer);
            }

            try
            {
                _customerRepository.Update(customer);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the customer. Please try again.");
                return View(customer);
            }
        }

        public ActionResult Delete(int id)
        {
            return View(_customerRepository.Get(id));
        }

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
