using System.ComponentModel;

namespace DPL.ViewModels.Auth.Role
{
	public class RoleVM
	{
		[DisplayName("Role Id")]
		public string Id { get; set; }

		[DisplayName("Role Name")]
		public string Name { get; set; }
	}
}
