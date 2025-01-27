using CarRentals.Data.Models;
using CarRentals.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CarRentals.Data.Service
{
    public class BookingService
    {
        private readonly ICarRepository _carRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;

        public BookingService(ICarRepository carRepository, ICustomerRepository customerRepository, IBookingRepository bookingRepository)
        {
            _carRepository = carRepository;
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
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
            // Check if the start date is in the past
            if (startDate.Date < DateTime.Now.Date) // Compare only the date part
            {
                return false; // Invalid if start date is before today
            }

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
                throw new Exception("Bilen hittades inte.");
            }

            // Check if customer exists
            var customer =_customerRepository.Get(model.CustomerId);
            if (customer == null)
            {
                throw new Exception("Kunden kunde inte hittas.");
            }

            // Validate the booking dates
            if (!IsDateValid(model.StartDate, model.EndDate))
            {
                throw new Exception("Ogiltiga datum: Se till att startdatumet är före slutdatumet och att startdatumet inte redan har passerat.");
            }

            // Calculate the number of rental days
            var rentalDuration = (model.EndDate - model.StartDate).Days;

            if (rentalDuration <= 0)
            {
                throw new Exception("Slutdatumet måste vara senare än startdatumet.");
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
        public bool IsCarAvailable(int carId, DateTime startDate, DateTime endDate)
        {
            // Check if there are any existing bookings for this car within the selected date range
            var existingBookings = _bookingRepository.GetBookingsByCarIdAndDateRange(carId, startDate, endDate);

            return !existingBookings.Any(); // If no overlapping bookings are found, the car is available
        }

        public IEnumerable<Car> GetAvailableCars(DateTime startDate, DateTime endDate)
        {
            var allCars = _carRepository.GetAll();  // Get all cars from the repository
            var conflictingBookings = _bookingRepository.GetBookingsByDateRange(startDate, endDate);  // Get all bookings in the given date range

            var availableCars = allCars.Where(car => car.IsAvailable && // Check if the car is available
                !conflictingBookings
                .Any(booking => booking.CarId == car.Id &&
                    ((booking.StartDate >= startDate && booking.StartDate <= endDate) ||  // Booking starts during the given range
                     (booking.EndDate >= startDate && booking.EndDate <= endDate) ||  // Booking ends during the given range
                     (booking.StartDate <= startDate && booking.EndDate >= endDate))) // Booking completely overlaps the given range
            ).ToList();

            return availableCars;
        }


        public Booking CreateBooking(string bookingDates, int userId, int carId)
        {
            // Split the booking dates string into start and end dates
            var dates = bookingDates.Split(" till ");
            if (dates.Length != 2)
            {
                throw new Exception("Ogiltigt format för bokningsdatum.");
            }

            var startDate = DateTime.Parse(dates[0]);
            var endDate = DateTime.Parse(dates[1]);

            // Validate the booking dates
            if (!IsDateValid(startDate, endDate))
            {
                throw new Exception("Ogiltiga datum: Se till att startdatumet är före slutdatumet och att startdatumet inte redan har passerat.");
            }

            // Check if car exists
            var car = _carRepository.Get(carId);
            if (car == null)
            {
                throw new Exception("Bilen hittades inte.");
            }

            // Check if customer exists
            var customer = _customerRepository.Get(userId);
            if (customer == null)
            {
                throw new Exception("Kunden kunde inte hittas.");
            }

            // Check if the car is available for the selected date range
            if (!IsCarAvailable(carId, startDate, endDate))
            {
                throw new Exception("Bilen är redan bokad för de valda datumen.");
            }

            // Calculate the number of rental days
            var rentalDurationInHours = (endDate - startDate).TotalHours;
            var rentalDurationInDays = (int)Math.Ceiling(rentalDurationInHours / 24);

            // Calculate the total price based on PricePerDay and rental duration (rounded up)
            var totalPrice = car.PricePerDay * rentalDurationInDays;

            // Create a new booking
            var booking = new Booking
            {
                CustomerId = userId,
                CarId = carId,
                StartDate = startDate,
                EndDate = endDate,
                TotalCost = totalPrice,
                IsCancelled = false // You can adjust this based on your logic
            };


            return _bookingRepository.Add(booking);

        }


    }
}
