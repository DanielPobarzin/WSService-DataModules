using WebAPI.Middlewares;

namespace WebAPI.Extensions
{
	public static class SwaggerExtensions
	{
		public static void UseSwaggerExtension(this IApplicationBuilder app)
		{
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service.Managment.WebApi");
			});
		}
		public static void UseErrorHandlingMiddleware(this IApplicationBuilder app)
		{
			app.UseMiddleware<ErrorHandlerMiddleware>();
		}
	}
}
