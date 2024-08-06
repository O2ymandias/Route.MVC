using DAL.Entities.BusinessModule;

namespace BLL.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
	{
		Task<IReadOnlyList<T>> GetAllAsync();
		Task<T?> GetAsync(int id);

		Task<IReadOnlyList<T>> GetAllWithSpecs(ISpecification<T> specification);
		Task<T?> GetWithSpecs(ISpecification<T> specification);

		void Add(T entity);
		void Update(T entity);
		void Delete(T entity);
	}
}
