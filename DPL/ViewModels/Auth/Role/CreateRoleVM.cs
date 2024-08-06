using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DPL.ViewModels.Auth.Role
{
	public class CreateRoleVM
	{
		[Required, DisplayName("Role Name")]
		public string RoleName { get; set; }
	}
}
