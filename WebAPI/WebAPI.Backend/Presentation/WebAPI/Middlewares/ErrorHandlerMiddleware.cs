using Application.Exceptions;
using Application.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Net;
using System.Text.Json;

namespace WebAPI.Middlewares
{
	public class ErrorHandlerMiddleware
	{
		private readonly RequestDelegate _next;

		public ErrorHandlerMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception error)
			{
				var code = HttpStatusCode.InternalServerError;
				var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message };

				switch (error)
				{
					case ValidationException validationException:
						code = HttpStatusCode.BadRequest;
						responseModel.Errors = validationException.Errors;
						break;
					case APIException:
						code = HttpStatusCode.BadRequest;
						break;
					case KeyNotFoundException:
						code = HttpStatusCode.NotFound;
						break;
				}
				Log.Error(error.Message);
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int)code;
				var result = JsonSerializer.Serialize(responseModel);

				if (result == string.Empty)
				{
					result = JsonSerializer.Serialize(new { error = error.Message });
				}

				await context.Response.WriteAsync(result);
			}
		}
	}
}
