using Microsoft.AspNetCore.Identity;

namespace DAL.Entities.IdentityModule
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public bool AgreeOnTerms { get; set; }
	}
}
