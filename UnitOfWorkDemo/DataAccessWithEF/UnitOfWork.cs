using DataAccess.Interfaces;
using DataAccessWithEF;

namespace DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UnitOfWorkDemoDbContext _dbContext; 

        /*private ILogRepository _logRepository;

        public ILogRepository LogEvents
        {
            get
            {
                _logRepository ??= new LogRepository(_dbContext);
                return _logRepository;
            }
        }*/

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
