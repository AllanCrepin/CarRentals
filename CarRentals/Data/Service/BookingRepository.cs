using CarRentals.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentals.Data.Service
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override IEnumerable<Booking> GetAll()
        {
            return _dbContext.Set<Booking>().OrderBy(b=>b.StartDate).ToList();
        }
        public IEnumerable<Booking> GetBookingsByCarIdAndDateRange(int carId, DateTime startDate, DateTime endDate)
        {
            return _dbContext.Bookings
                .Where(b => b.CarId == carId && ((b.StartDate < endDate && b.EndDate > startDate)))
                .ToList();
        }

        public IEnumerable<Booking> GetBookingsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _dbContext.Bookings
                .Where(booking => booking.StartDate < endDate && booking.EndDate > startDate)
                .ToList();
        }

        public IEnumerable<Booking> EagerGetAll()
        {
            return _dbContext.Bookings
                           .Include(b => b.Customer)
                           .Include(b => b.Car)
                           .ToList();
        }

        public IEnumerable<Booking> GetAllByCustomerId(int customerId)
        {
            return _dbContext.Bookings
                           .Include(b => b.Customer)
                           .Include(b => b.Car)
                           .Where(b => b.CustomerId == customerId)
                           .OrderBy(b => b.StartDate)
                           .ToList();
        }
    }
}
