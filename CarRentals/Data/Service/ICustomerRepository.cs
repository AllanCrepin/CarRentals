using CarRentals.Data.Models;

namespace CarRentals.Data.Service
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        public Customer GetByEmail(string email);
    }
}
