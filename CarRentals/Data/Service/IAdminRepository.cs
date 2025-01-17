using CarRentals.Data.Models;

namespace CarRentals.Data.Service
{
    public interface IAdminRepository : IRepository<Admin>
    {
        public Admin GetByEmail(string email);
    }
}
