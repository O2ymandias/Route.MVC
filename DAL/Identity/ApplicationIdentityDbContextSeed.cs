using DAL.Entities.IdentityModule;
using DAL.Entities.IdentityModule.Helpers;
using Microsoft.AspNetCore.Identity;

namespace DAL.Identity
{
	public static class ApplicationIdentityDbContextSeed
	{
		public static async Task SeedIdentityData(UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			if (!roleManager.Roles.Any())
			{
				var adminRole = new IdentityRole() { Name = RoleConstants.Admin };
				var userRole = new IdentityRole() { Name = RoleConstants.User };

				await roleManager.CreateAsync(adminRole);
				await roleManager.CreateAsync(userRole);
			}

			if (!userManager.Users.Any())
			{
				var admin = new ApplicationUser()
				{
					FirstName = string.Empty,
					LastName = string.Empty,
					UserName = "ApplicationAdmin",
					Email = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
					PhoneNumber = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
				};
				await userManager.CreateAsync(admin, "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");

				await userManager.AddToRoleAsync(admin, RoleConstants.Admin);

			}

		}

	}
}
