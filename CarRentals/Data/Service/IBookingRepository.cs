using CarRentals.Data.Models;

namespace CarRentals.Data.Service
{
    public interface IBookingRepository : IRepository<Booking>
    {
        IEnumerable<Booking> GetBookingsByCarIdAndDateRange(int carId, DateTime startDate, DateTime endDate);
    }

}
