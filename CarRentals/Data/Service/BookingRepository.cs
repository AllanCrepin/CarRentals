using CarRentals.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentals.Data.Service
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public IEnumerable<Booking> GetBookingsByCarIdAndDateRange(int carId, DateTime startDate, DateTime endDate)
        {
            return _dbContext.Bookings
                .Where(b => b.CarId == carId &&
                            ((b.StartDate < endDate && b.EndDate > startDate))) // Check for overlapping dates
                .ToList();
        }

        public IEnumerable<Booking> EagerGetAll()
        {
            return _dbContext.Bookings
                           .Include(b => b.Customer)  // Eager load Customer
                           .Include(b => b.Car)      // Eager load Car
                           .ToList();  // Immediately execute the query
        }

        // Eager loading method with filtering by CustomerId
        public IEnumerable<Booking> GetAllByCustomerId(int customerId)
        {
            return _dbContext.Bookings
                           .Include(b => b.Customer)  // Eager load Customer
                           .Include(b => b.Car)       // Eager load Car
                           .Where(b => b.CustomerId == customerId)  // Filter by CustomerId
                           .OrderBy(b => b.StartDate)
                           .ToList();  // Execute the query and return a list
        }
    }
}
