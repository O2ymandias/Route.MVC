using DAL.Entities.BusinessModule;

namespace BLL.Specifications.DepartmentSpecs
{
    public class DepartmentSpecifications : BaseSpecification<Department>
	{
		public DepartmentSpecifications(string? departmentName)
			: base(department =>
			string.IsNullOrEmpty(departmentName) || department.Name.ToLower().Equals(departmentName.ToLower()))
		{
		}
	}
}
