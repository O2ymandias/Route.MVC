using DPL.CustomAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DPL.ViewModels
{
	public class DepartmentVM
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Code Is Required")]
		public string Code { get; set; }

		[Required]
		[MaxLength(100)]
		[UniqueName]
		public string Name { get; set; }

		[Required]
		[DisplayName("Date Of Creation")]
		[DataType(DataType.DateTime)]
		public DateTime DateOfCreation { get; set; }
	}
}
