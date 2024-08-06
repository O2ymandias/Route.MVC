namespace DPL.Services.Helpers
{
	public class JwtSettings
	{
		public static string JwtTokenKey => "JWT";
		public string Issuer { get; set; }
		public string Audience { get; set; }
		public int DaysBeforeExpiry { get; set; }
		public string SecurityKey { get; set; }
	}
}
