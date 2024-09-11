using DataAccess.Repositories;
using DataAccessWithEF.Interfaces.Repositories;
using DataAccessWithEF.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace DataAccessWithEF.Repositories
{
    public class BlogRepository : Repository<Blog>, IBlogRepository
    {
        public BlogRepository(UnitOfWorkDemoDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<Blog>> GetBlogsWithUserAndComments(
            Expression<Func<Blog, bool>>? predicate = null,
            Func<IQueryable<Blog>, IOrderedQueryable<Blog>>? orderBy = null,
            int? count = null)
        {
            IQueryable<Blog> query = _dbContext
                .Set<Blog>()
                .Include(b => b.CreatedByUser)
                .Include(b => b.Comments);

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            if (orderBy is not null)
            {
                query = orderBy(query);
            }

            if (count is not null && count > 0)
            {
                query = query.Take(count.Value);
            }

            return await query.ToListAsync();
        }
    }
}
