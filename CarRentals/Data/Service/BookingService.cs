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
            if (startDate.Date < DateTime.Now.Date)
            {
                return false;
            }

            if (startDate >= endDate)
            {
                return false;
            }

            var durationInHours = (endDate - startDate).TotalHours;
            if (durationInHours < 2)
            {
                return false;
            }

            return true;
        }

        public bool IsCarAvailable(int carId, DateTime startDate, DateTime endDate)
        {
            var existingBookings = _bookingRepository.GetBookingsByCarIdAndDateRange(carId, startDate, endDate);

            return !existingBookings.Any();
        }

        public IEnumerable<Car> GetAvailableCars(DateTime startDate, DateTime endDate)
        {
            var bookedCarIds = _bookingRepository
                .GetBookingsByDateRange(startDate, endDate)
                .Where(b => b.StartDate <= endDate && b.EndDate >= startDate)
                .Select(b => b.CarId)
                .ToHashSet();

            return _carRepository.GetAll()
                .Where(car => car.IsAvailable && !bookedCarIds.Contains(car.Id) &&
                    !_bookingRepository.GetBookingsByDateRange(startDate.AddDays(-1), startDate.AddDays(-1))
                .Any(b => b.CarId == car.Id));
        }


        public Booking CreateBooking(string bookingDates, int userId, int carId)
        {
            var dates = bookingDates.Split(" till ");
            if (dates.Length != 2)
            {
                throw new Exception("Ogiltigt format för bokningsdatum.");
            }

            var startDate = DateTime.Parse(dates[0]);
            var endDate = DateTime.Parse(dates[1]);

            if (!IsDateValid(startDate, endDate))
            {
                throw new Exception("Ogiltiga datum: Se till att startdatumet är före slutdatumet och att startdatumet inte redan har passerat.");
            }

            var car = _carRepository.Get(carId);
            if (car == null)
            {
                throw new Exception("Bilen hittades inte.");
            }

            var customer = _customerRepository.Get(userId);
            if (customer == null)
            {
                throw new Exception("Kunden kunde inte hittas.");
            }

            if (!IsCarAvailable(carId, startDate, endDate))
            {
                throw new Exception("Bilen är redan bokad för de valda datumen.");
            }

            var rentalDurationInHours = (endDate - startDate).TotalHours;
            var rentalDurationInDays = (int)Math.Ceiling(rentalDurationInHours / 24);

            var totalPrice = car.PricePerDay * rentalDurationInDays;

            var booking = new Booking
            {
                CustomerId = userId,
                CarId = carId,
                StartDate = startDate,
                EndDate = endDate,
                TotalCost = totalPrice,
                IsCancelled = false
            };

            return _bookingRepository.Add(booking);

        }


    }
}
