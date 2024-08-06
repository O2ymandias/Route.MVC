using BLL.Interfaces;
using DAL.Entities.BusinessModule;
using Microsoft.EntityFrameworkCore;

namespace BLL.Specifications
{
    public static class SpecificationEvaluator<T> where T : BaseEntity
	{
		public static IQueryable<T> BuildQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
		{
			var query = inputQuery;
			if (specification.Criteria is not null)
				query = query.Where(specification.Criteria);

			if (specification.Includes.Count > 0)
				query = specification.Includes.Aggregate(query, (current, next) => current.Include(next));

			return query;
		}
	}
}
