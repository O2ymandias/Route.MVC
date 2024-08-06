using DAL.Entities.BusinessModule;
using System.Linq.Expressions;

namespace BLL.Interfaces
{
    public interface ISpecification<T> where T : BaseEntity
	{
		Expression<Func<T, bool>> Criteria { get; set; }
		List<Expression<Func<T, object>>> Includes { get; set; }
	}
}
