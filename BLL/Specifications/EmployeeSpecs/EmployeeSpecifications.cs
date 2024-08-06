using DAL.Entities.BusinessModule;

namespace BLL.Specifications.EmployeeSpecs
{
    public class EmployeeSpecifications : BaseSpecification<Employee>
	{
		public EmployeeSpecifications()
		{
			Includes.Add(e => e.Department);
		}

		public EmployeeSpecifications(int employeeId)
			: base(employee => employee.Id == employeeId)
		{
			Includes.Add(e => e.Department);
		}

		public EmployeeSpecifications(string? search)
			: base(employee => string.IsNullOrEmpty(search) || employee.Name.Contains(search))
		{
			Includes.Add(e => e.Department);
		}
	}
}
