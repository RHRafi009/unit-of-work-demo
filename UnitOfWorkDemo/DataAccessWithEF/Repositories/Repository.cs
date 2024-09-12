using DataAccess.Interfaces.Repositories;
using DataAccessWithEF;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly UnitOfWorkDemoDbContext _dbContext;

        public Repository(UnitOfWorkDemoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entities);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate, 
            int count, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy
            )
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>().Where(predicate);
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.Take(count).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }
    }
}
