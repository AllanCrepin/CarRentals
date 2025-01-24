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
    }
}
