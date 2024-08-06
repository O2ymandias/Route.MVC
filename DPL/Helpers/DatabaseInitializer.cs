using DAL.Entities.IdentityModule;
using DAL.Identity;
using Microsoft.AspNetCore.Identity;

namespace DPL.Helpers
{
	public static class DatabaseInitializer
	{
		public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication webApplication)
		{
			using var scope = webApplication.Services.CreateScope();
			var scopedServices = scope.ServiceProvider;

			var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();
			var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();

			await ApplicationIdentityDbContextSeed.SeedIdentityData(userManager, roleManager);

			return webApplication;
		}
	}
}
