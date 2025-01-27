using CarRentals.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CarRentals.Data.Service
{
    public class CarRepository : GenericRepository<Car>, ICarRepository
    {
        public CarRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<Car> GetAllAvailable()
        {
            return _dbContext.Cars.Where(c => c.IsAvailable == true).ToList();
        }
    }
}
