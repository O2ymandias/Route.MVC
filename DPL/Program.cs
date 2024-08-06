using DPL.CustomMiddlewares;
using DPL.Helpers;
using DPL.Services.Extensions;

namespace DPL
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			#region Add services to the IOC container

			/*
            In ASP.NET Core, dependency injection is a technique to achieve Inversion of Control (IoC) between classes and their dependencies.
            The framework provides three primary methods to register services with the built-in IoC container:
            AddTransient(), AddScoped(), and AddSingleton().
            These methods define the service lifetime and behavior within the application's lifecycle.

            *****************************************************************************************
            *****************************************************************************************
            *
            Common Services Used with AddTransient()
                - Creates a new instance of the service each time it is requested.
                This is useful for lightweight, stateless services such as:

                    1. Utility Classes: Classes that perform operations like string manipulations, file I/O, auto mapper, etc.
                    2. Services with No State: Stateless services that don't retain any information between calls.


            Common Services Used with AddScoped()
                - Creates a new instance of the service per client request (connection).
                This is useful for services that maintain state within a single request but are not shared across requests such as:

                    1. Database Contexts: Typically, Entity Framework (EF) DbContext instances are scoped
                    because they manage the database connection and track changes within a request.
                    2. Business Logic Services (Repositories): Services that implement business rules and operations related to the current HTTP request.
                    3. Unit of Work : that maintain a list of operations to be performed in a single transaction.


            Common Services Used with AddSingleton()
                - Creates a single instance of the service for the lifetime of the application.
                This is useful for services that are expensive to create and should be shared across all requests such as:

                    1. Configuration Services: Services that manage configuration settings that do not change throughout the application lifetime.
                    2. Caching Services: Services that store and retrieve data that should be shared across the application to improve performance.
                    3. Logging Services: Services that handle logging and need to be available throughout the application's lifetime.
             */

			builder.Services.AddControllersWithViews();

			builder.Services
				.AddApplicationServices(builder.Configuration)
				.AddApplicationIdentityServices(builder.Configuration);

			#endregion

			var app = builder.Build();

			await app.InitializeDatabaseAsync();

			#region Configure the HTTP request pipeline.

			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseMiddleware<UnauthorizedMiddleware>();

			app.UseAuthentication();

			app.UseAuthorization();


			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();

			#endregion
		}
	}
}
