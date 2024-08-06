using BLL;
using BLL.Interfaces;
using DAL.Data;
using DAL.Entities.IdentityModule;
using DAL.Identity;
using DPL.CustomMiddlewares;
using DPL.Helpers;
using DPL.Services.Contract;
using DPL.Services.Helpers;
using DPL.Services.Implementation;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DPL.Services.Extensions
{
	public static class ApplicationServicesExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services,
			IConfiguration configuration)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options
				.UseLazyLoadingProxies()
				.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});
			services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
			services.AddAutoMapper(typeof(MappingProfiles));
			services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
			services.AddTransient(typeof(IEmailSender), typeof(EmailSender));
			services.AddScoped(typeof(UnauthorizedMiddleware));
			services.Configure<TwilioSettings>(configuration.GetSection("TwilioSettings"));
			services.AddTransient(typeof(ISmsSender), typeof(SmsSender));

			return services;
		}

		public static IServiceCollection AddApplicationIdentityServices(this IServiceCollection services,
			IConfiguration configuration)
		{
			services.AddDbContext<ApplicationIdentityDbContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"));
			});

			services.Configure<JwtSettings>(configuration.GetSection("JWT"));
			services.AddScoped(typeof(ITokenService), typeof(TokenService));

			services
				.AddIdentity<ApplicationUser, IdentityRole>(setup =>
				{
					setup.Password.RequireUppercase = true;
					setup.Password.RequireLowercase = true;
					setup.Password.RequireNonAlphanumeric = true;
					setup.Password.RequiredUniqueChars = 1;
					setup.Password.RequiredLength = 6;

					setup.Lockout.AllowedForNewUsers = true;
					setup.Lockout.MaxFailedAccessAttempts = 5;
					setup.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

					setup.User.RequireUniqueEmail = true;
				})
				.AddEntityFrameworkStores<ApplicationIdentityDbContext>()
				.AddDefaultTokenProviders();

			#region Using "Application.Identity" Scheme

			/*
			 * When Using AddIdentity(), AddAuthentication Is Called Internally To Configure:
				*  DefaultAuthenticateScheme
				*  DefaultChallengeScheme
				*  
			
			[ASP.NETCORE Code]
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
				options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
				options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
			})
			.AddCookie(IdentityConstants.ApplicationScheme, o =>
			{
				o.LoginPath = new PathString("/Account/Login");
				o.Events = new CookieAuthenticationEvents
				{
					OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
				};
			});
			 */

			/*
			// Configuration Of The Current Default Scheme => "Identity.Application"
			services.ConfigureApplicationCookie(config =>
			{
				config.LoginPath = new PathString("/Account/Login");
				config.LogoutPath = new PathString("/Home/Index");
				config.AccessDeniedPath = new PathString("/Home/Error");
				config.ExpireTimeSpan = TimeSpan.FromDays(14);

			}); 
			*/

			#endregion

			#region  Using JWT


			services
				.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters()
					{
						ValidateIssuer = true,
						ValidIssuer = configuration["JWT:Issuer"],

						ValidateAudience = true,
						ValidAudience = configuration["JWT:Audience"],

						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecurityKey"]!)),

						ValidateLifetime = true,
					};

					/*
					- By default, the JWT bearer authentication expects the token to be in the Authorization header of the HTTP request.
					- By overriding the OnMessageReceived event, you ensure that the authentication middleware looks for the JWT token in the cookie
					allowing your application to seamlessly support token-based authentication
					without requiring tokens to be sent in headers.
					 */
					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							var token = context.Request.Cookies[JwtSettings.JwtTokenKey];
							if (!string.IsNullOrEmpty(token))
							{
								context.Token = token;
							}
							return Task.CompletedTask;
						}
					};
				});

			#endregion

			#region External Login With Google

			services
				.AddAuthentication()
				.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
				{
					var googleSection = configuration.GetSection("Authentication:Google");
					options.ClientId = googleSection["ClientId"]!;
					options.ClientSecret = googleSection["ClientSecret"]!;
				});

			#endregion

			return services;
		}
	}
}
