using BLL.Interfaces;
using BLL.Specifications;
using DAL.Data;
using DAL.Entities.BusinessModule;
using Microsoft.EntityFrameworkCore;

namespace BLL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
	{
		private readonly ApplicationDbContext _dbContext;

		public GenericRepository(ApplicationDbContext dbContext)
			=> _dbContext = dbContext;


		public async Task<IReadOnlyList<T>> GetAllAsync()
			=> await _dbContext.Set<T>().AsNoTracking().ToListAsync();

		public async Task<T?> GetAsync(int id)
			=> await _dbContext.Set<T>().FindAsync(id);

		public void Add(T entity)
			=> _dbContext.Set<T>().Add(entity);

		public void Update(T entity)
			=> _dbContext.Set<T>().Update(entity);

		public void Delete(T entity)
			=> _dbContext.Set<T>().Remove(entity);

		public async Task<IReadOnlyList<T>> GetAllWithSpecs(ISpecification<T> specification)
			=> await ApplySpecification(specification).ToListAsync();

		public async Task<T?> GetWithSpecs(ISpecification<T> specification)
			=> await ApplySpecification(specification).FirstOrDefaultAsync();


		private IQueryable<T> ApplySpecification(ISpecification<T> specification)
			=> SpecificationEvaluator<T>.BuildQuery(_dbContext.Set<T>(), specification);
	}
}
