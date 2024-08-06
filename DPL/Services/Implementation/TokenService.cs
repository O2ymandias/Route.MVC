using DAL.Entities.IdentityModule;
using DPL.Services.Contract;
using DPL.Services.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DPL.Services.Implementation
{
	public class TokenService : ITokenService
	{
		private readonly JwtSettings _jwtSettings;
		private readonly UserManager<ApplicationUser> _userManager;

		public TokenService(IOptions<JwtSettings> jwtSettingsOptions,
			UserManager<ApplicationUser> userManager)
		{
			_jwtSettings = jwtSettingsOptions.Value;
			_userManager = userManager;
		}


		public async Task<string> GenerateTokenAsync(ApplicationUser applicationUser)
		{
			var privateClaims = new List<Claim>()
			{
				new(ClaimTypes.Name, applicationUser.UserName ?? string.Empty),
				new(ClaimTypes.Email, applicationUser.Email ?? string.Empty),
			};

			var roles = await _userManager.GetRolesAsync(applicationUser);
			if (roles.Count > 0)
				foreach (var role in roles)
					privateClaims.Add(new Claim(ClaimTypes.Role, role));

			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));

			var token = new JwtSecurityToken(issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				claims: privateClaims,
				expires: DateTime.UtcNow.AddDays(_jwtSettings.DaysBeforeExpiry),
				signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

			var tokenHandler = new JwtSecurityTokenHandler();
			return tokenHandler.WriteToken(token);
		}
	}
}
