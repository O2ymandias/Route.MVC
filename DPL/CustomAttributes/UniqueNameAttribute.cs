using BLL.Interfaces;
using BLL.Specifications.DepartmentSpecs;
using DAL.Entities.BusinessModule;
using System.ComponentModel.DataAnnotations;

namespace DPL.CustomAttributes
{
    public class UniqueNameAttribute : ValidationAttribute
	{
		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		// Only Works Server-Side Validation
		// Tag-Helper(asp-for) Won't Generate Html Attributes For It
		// So JQuery-Validation-Unobtrusive Won't Validate On It
		{
			var name = value as string;

			var unitOfWork = validationContext.GetRequiredService<IUnitOfWork>();
			var specs = new DepartmentSpecifications(name);
			var departments = unitOfWork.Repository<Department>().GetAllWithSpecs(specs).Result;

			return departments.Count == 0
				? ValidationResult.Success
				: new ValidationResult("Department Name Must Be Unique");
		}
	}
}
