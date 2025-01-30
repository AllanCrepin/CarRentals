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