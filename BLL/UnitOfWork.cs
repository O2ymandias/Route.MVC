using BLL.Interfaces;
using BLL.Repositories;
using DAL.Data;
using DAL.Entities.BusinessModule;
using System.Collections;

namespace BLL
{
    public class UnitOfWork : IUnitOfWork
	{
		private readonly Hashtable _repositories;
		private readonly ApplicationDbContext _dbContext;

		public UnitOfWork(ApplicationDbContext dbContext)
		{
			_repositories = new Hashtable();
			_dbContext = dbContext;
		}

		public IGenericRepository<T> Repository<T>() where T : BaseEntity
		{
			var key = typeof(T).Name;
			if (!_repositories.ContainsKey(key))
			{
				var repo = new GenericRepository<T>(_dbContext);
				_repositories.Add(key, repo);
			}
			return (_repositories[key] as IGenericRepository<T>)!;
		}

		public async Task<int> CompleteAsync()
			=> await _dbContext.SaveChangesAsync();

		public async ValueTask DisposeAsync()
			=> await _dbContext.DisposeAsync();

	}
}
