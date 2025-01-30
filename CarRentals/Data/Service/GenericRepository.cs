using CarRentals.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CarRentals.Data.Service
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;
        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual T Get(int id)
        {
            return _dbContext.Find<T>(id);
        }

        public virtual T Add(T entity)
        {
            var addedEntity = _dbContext.Add(entity).Entity;
            _dbContext.SaveChanges();
            return addedEntity;
        }

        public virtual T Update(T entity)
        {
            var updatedEntity = _dbContext.Update(entity).Entity;
            _dbContext.SaveChanges();
            return updatedEntity;

        }

        public virtual void Delete(int id)
        {
            var entity = Get(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
                _dbContext.SaveChanges();
            }
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().AsQueryable().Where(predicate).ToList();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>().ToList();
        }

        public virtual bool Exists(int id)
        {
            var entity = Get(id);
            return entity != null;
        }

        public virtual void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

    }
}