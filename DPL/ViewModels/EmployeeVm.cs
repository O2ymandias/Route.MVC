using DAL.Entities.BusinessModule;
using DPL.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DPL.ViewModels
{
    public class EmployeeVm
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(50, ErrorMessage = "Name Must Not Exceed 50 Character Length")]
		[MinLength(5, ErrorMessage = "Name Must Be Atleast 5 Character Length")]
		public string Name { get; set; }

		[Required]
		[Range(20, 60)]
		public int Age { get; set; }

		[Required]
		public string Address { get; set; }

		[Required]
		[DataType(DataType.Currency)]
		[Remote(action: nameof(EmployeesController.CheckSalary),
			controller: "Employees",
			ErrorMessage = "Salary Must Be Greater Than Or Equal To $5000.00")]
		/*
			- TagHelpers Will Generate Html Attributes For It
				data-val-remote
				data-val-remote-additionalfields
				data-val-remote-url
			
			- JQuery-Validation-Unobtrusive Will Validate On It
			
			- With Every Text Change At Salary Field,
			AJAX Call Will Happen To Invoke CheckSalary() Method, To Validate On Salary
			- Form Will Communicate With The Server Without Reloading
		 */
		public decimal Salary { get; set; }

		[Required]
		[DisplayName("Is Active")]
		public bool IsActive { get; set; }

		[Required]
		[Phone]
		[DisplayName("Phone Number")]
		public string PhoneNumber { get; set; }

		[Required]
		[EmailAddress]
		[DisplayName("Email Address")]
		public string EmailAddress { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		[DisplayName("Hiring Date")]
		public DateTime HiringDate { get; set; }

		[Required]
		public Gender Gender { get; set; }

		[Required]
		public EmployeeType EmployeeType { get; set; }

		public Department? Department { get; set; }
		public int? DepartmentId { get; set; }

		public string? ImageName { get; set; }
		public IFormFile? Image { get; set; }
	}
}
