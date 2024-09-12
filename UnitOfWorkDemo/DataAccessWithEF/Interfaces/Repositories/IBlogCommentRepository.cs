using DataAccess.Interfaces.Repositories;
using DataAccessWithEF.Models;

namespace DataAccessWithEF.Interfaces.Repositories
{
    public interface IBlogCommentRepository : IRepository<BlogComment>
    {
    }
}
