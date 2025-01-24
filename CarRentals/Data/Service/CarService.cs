using CarRentals.Data.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace CarRentals.Data.Service
{
    public class CarService
    {
        private readonly ICarRepository _carRepository;
        private readonly IBookingRepository _bookingRepository;

        public CarService(ICarRepository carRepository, IBookingRepository bookingRepository)
        {
            _carRepository = carRepository;
            _bookingRepository = bookingRepository;
        }

        public List<Car> GetMostBookedCars(int count)
        {
            // Filter only available cars and calculate booking count
            var topBookedCars = _carRepository.GetAll()
                .Where(car => car.IsAvailable) // Only include available cars
                .Select(car => new
                {
                    Car = car,
                    BookingCount = _bookingRepository.GetAll()
                        .Count(b => b.CarId == car.Id && !b.IsCancelled) // Count bookings for this car
                })
                .OrderByDescending(c => c.BookingCount) // Order by booking count descending
                .Take(count) // Take the top 'count' cars
                .Select(c => c.Car)
                .ToList();

            // Fallback logic if less than 'count' available cars are found
            if (topBookedCars.Count < count)
            {
                var fallbackCars = _carRepository.GetAll()
                    .Where(car => car.IsAvailable) // Only include available cars for fallback
                    .Take(count)
                    .ToList();

                // Merge the two lists, ensuring no duplicates
                topBookedCars = topBookedCars
                    .Concat(fallbackCars)
                    .Distinct()
                    .Take(count)
                    .ToList();
            }

            return topBookedCars;
        }
    }
}
