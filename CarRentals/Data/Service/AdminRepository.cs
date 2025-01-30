using CarRentals.Data.Models;
using System.Linq.Expressions;

namespace CarRentals.Data.Service
{
    public class AdminRepository : GenericRepository<Admin>, IAdminRepository
    {
        public AdminRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public Admin GetByEmail(string email)
        {
            return _dbContext.Set<Admin>().FirstOrDefault(a => a.Email == email);
            
        }
    }
    
}
