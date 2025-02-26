﻿using CarRentals.Data.Models;

namespace CarRentals.Data.Service
{
    public interface IBookingRepository : IRepository<Booking>
    {
        IEnumerable<Booking> GetBookingsByCarIdAndDateRange(int carId, DateTime startDate, DateTime endDate);

        IEnumerable<Booking> GetBookingsByDateRange(DateTime startDate, DateTime endDate);

        IEnumerable<Booking> EagerGetAll();

        IEnumerable<Booking> GetAllByCustomerId(int customerId);
    }

}
