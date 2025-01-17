using System.Linq.Expressions;

namespace CarRentals.Data.Service
{
    public interface IRepository<T>
    {
        T Add(T entity);
        T Update(T entity);
        T Get(int id);
        IEnumerable<T> GetAll();
        void Delete(int id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        bool Exists(int id);

        void SaveChanges();
    }
}



/*
 
 MARCUS:

public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task UpdateAsync(T entity);
        Task<T> AddAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
 
 
 
 */