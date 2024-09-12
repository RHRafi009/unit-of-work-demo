using DataAccess.Repositories;
using DataAccessWithEF.Interfaces.Repositories;
using DataAccessWithEF.Models;

namespace DataAccessWithEF.Repositories
{
    public class BlogCommentRepository : Repository<BlogComment>, IBlogCommentRepository
    {
        public BlogCommentRepository(UnitOfWorkDemoDbContext dbContext) : base(dbContext)
        {
        }
    }
}
