using DAL.Entities.IdentityModule;

namespace DPL.Services.Contract
{
	public interface ITokenService
	{
		Task<string> GenerateTokenAsync(ApplicationUser applicationUser);
	}
}
