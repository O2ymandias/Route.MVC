
namespace DPL.CustomMiddlewares
{
	public class UnauthorizedMiddleware : IMiddleware
	{
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			await next.Invoke(context); // Invoking Next Middleware

			// This Code Will Only Be Executed When The Response Is Returned 
			if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
				context.Response.Redirect("/Account/Login");
		}
	}
}
