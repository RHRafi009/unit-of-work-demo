using DataAccess.Repositories;
using DataAccessWithEF.Interfaces.Repositories;
using DataAccessWithEF.Models;

namespace DataAccessWithEF.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(UnitOfWorkDemoDbContext dbContext) : base(dbContext)
        {
        }
    }
}
