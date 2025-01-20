using CarRentals.Data.Models;

namespace CarRentals.Data.Service
{

    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public Customer GetByEmail(string email)
        {
            return _dbContext.Set<Customer>().FirstOrDefault(a => a.Email == email);

        }
    }
    
}
