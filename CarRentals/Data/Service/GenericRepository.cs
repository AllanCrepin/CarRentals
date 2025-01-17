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
            //.ToList() is eager loading
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




/*
 MARCUS:

public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;

        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            await context.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            context.Update(entity);
            await context.SaveChangesAsync();
        }
    }
 
 
 





 */