using CarRentals.Data.Models;
using CarRentals.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CarRentals.Data.Service
{
    public class BookingService
    {
        private readonly ICarRepository _carRepository;
        private readonly ICustomerRepository _customerRepository;

        public BookingService(ICarRepository carRepository, ICustomerRepository customerRepository)
        {
            _carRepository = carRepository;
            _customerRepository = customerRepository;
        }

        public bool CarExists(int carId)
        {
            return _carRepository.Exists(carId);
        }

        public bool CustomerExists(int customerId)
        {
            return _customerRepository.Exists(customerId);
        }

        public bool IsDateValid(DateTime startDate, DateTime endDate)
        {
            // Check if the start date is after the end date
            if (startDate >= endDate)
            {
                return false; // Invalid if start date is after or the same as end date
            }

            // Check if the duration is less than 2 hours
            var durationInHours = (endDate - startDate).TotalHours;
            if (durationInHours < 2)
            {
                return false; // Invalid if duration is less than 2 hours
            }

            return true; // Valid if no issues
        }

        public Booking CreateBooking(BookingViewModel model)
        {
            // Check if car exists
            var car = _carRepository.Get(model.CarId);
            if (car == null)
            {
                throw new Exception("Car not found");
            }

            // Check if customer exists
            var customer =_customerRepository.Get(model.CustomerId);
            if (customer == null)
            {
                throw new Exception("Customer not found");
            }

            // Validate the booking dates
            if (!IsDateValid(model.StartDate, model.EndDate))
            {
                throw new Exception("Invalid dates: Ensure the start date is before the end date and the duration is at least 2 hours.");
            }

            // Calculate the number of rental days
            var rentalDuration = (model.EndDate - model.StartDate).Days;

            if (rentalDuration <= 0)
            {
                throw new Exception("End date must be later than the start date.");
            }


            // Calculate the total number of days for the booking
            var rentalDurationInHours = (model.EndDate - model.StartDate).TotalHours;
            // Round up the duration in hours to the nearest whole day
            var rentalDurationInDays = (int)Math.Ceiling(rentalDurationInHours / 24);

            // Calculate the total price based on PricePerDay and rental duration (rounded up)
            var totalPrice = car.PricePerDay * rentalDurationInDays;

            // Create a new booking
            var booking = new Booking
            {
                CustomerId = model.CustomerId,
                CarId = model.CarId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsCancelled = model.IsCancelled,
                TotalCost = totalPrice // Set the calculated total price
            };

            // Add the booking to the context and save
            /*
            _context.Add(booking);
            await _context.SaveChangesAsync();
            */

            return booking;
        }


    }
}
