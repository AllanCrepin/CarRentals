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

        public override IEnumerable<Car> Find(Expression<Func<Car, bool>> predicate)
        {
            return base.Find(predicate);
        }
    }
}
