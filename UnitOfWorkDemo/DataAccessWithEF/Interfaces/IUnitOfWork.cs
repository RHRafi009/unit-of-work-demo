namespace DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChnagesAsync();
        Task BeginTransactionAsync();
        Task CompleteTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
