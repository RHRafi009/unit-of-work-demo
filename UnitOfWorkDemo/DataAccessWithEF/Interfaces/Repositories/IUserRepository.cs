using DataAccess.Interfaces.Repositories;
using DataAccessWithEF.Models;

namespace DataAccessWithEF.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
    }
}
