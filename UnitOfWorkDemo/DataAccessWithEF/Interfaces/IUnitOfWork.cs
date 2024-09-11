using DataAccessWithEF.Interfaces.Repositories;

namespace DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBlogRepository BlogRepo { get; }

        Task<int> SaveChnagesAsync();
        Task BeginTransactionAsync();
        Task CompleteTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
