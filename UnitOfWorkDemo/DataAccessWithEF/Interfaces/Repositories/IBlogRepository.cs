using DataAccess.Interfaces.Repositories;
using DataAccessWithEF.Models;
using System.Linq.Expressions;

namespace DataAccessWithEF.Interfaces.Repositories
{
    public interface IBlogRepository : IRepository<Blog>
    {
        Task<IEnumerable<Blog>> GetBlogsWithUserAndComments(Expression<Func<Blog, bool>>? predicate = null, Func<IQueryable<Blog>, IOrderedQueryable<Blog>>? orderBy = null, int? count = null);
    }
}
