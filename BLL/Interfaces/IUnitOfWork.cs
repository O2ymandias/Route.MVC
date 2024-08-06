using DAL.Entities.BusinessModule;

namespace BLL.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
	{
		IGenericRepository<T> Repository<T>() where T : BaseEntity;
		Task<int> CompleteAsync();
	}
}
