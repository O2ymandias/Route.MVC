using BLL.Interfaces;
using DAL.Entities.BusinessModule;
using System.Linq.Expressions;

namespace BLL.Specifications
{
    public class BaseSpecification<T> : ISpecification<T> where T : BaseEntity
	{
		public BaseSpecification()
		{
			Includes = new List<Expression<Func<T, object>>>();
		}

		public BaseSpecification(Expression<Func<T, bool>> criteria)
		{
			Criteria = criteria;
			Includes = new List<Expression<Func<T, object>>>();
		}

		public Expression<Func<T, bool>> Criteria { get; set; }
		public List<Expression<Func<T, object>>> Includes { get; set; }
	}
}
