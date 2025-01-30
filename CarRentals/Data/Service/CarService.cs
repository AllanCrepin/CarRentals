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
            var topBookedCars = _carRepository.GetAll()
                .Where(car => car.IsAvailable)
                .Select(car => new
                {
                    Car = car,
                    BookingCount = _bookingRepository.GetAll()
                        .Count(b => b.CarId == car.Id && !b.IsCancelled)
                })
                .OrderByDescending(c => c.BookingCount)
                .Take(count)
                .Select(c => c.Car)
                .ToList();

            if (topBookedCars.Count < count)
            {
                var fallbackCars = _carRepository.GetAll()
                    .Where(car => car.IsAvailable)
                    .Take(count)
                    .ToList();

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
