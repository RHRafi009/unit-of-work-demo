using DataAccessWithEF.Interfaces.Repositories;

namespace DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBlogRepository BlogRepo { get; }
        IUserRepository UserRepo { get; }
        IBlogCommentRepository BlogCommentRepo { get; }

        Task<int> SaveChnagesAsync();
        Task BeginTransactionAsync();
        Task CompleteTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
