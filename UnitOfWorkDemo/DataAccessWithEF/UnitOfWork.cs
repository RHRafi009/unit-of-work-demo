using DataAccess.Interfaces;
using DataAccessWithEF;
using DataAccessWithEF.Interfaces.Repositories;
using DataAccessWithEF.Repositories;

namespace DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UnitOfWorkDemoDbContext _dbContext;

        private IBlogRepository _blogRepository;

        public IBlogRepository BlogRepo
        {

            get 
            {
                _blogRepository ??= new BlogRepository(_dbContext);
                return _blogRepository; 
            } 
        }

        public UnitOfWork(UnitOfWorkDemoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChnagesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CompleteTransactionAsync()
        {
            await _dbContext.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _dbContext.Database.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
