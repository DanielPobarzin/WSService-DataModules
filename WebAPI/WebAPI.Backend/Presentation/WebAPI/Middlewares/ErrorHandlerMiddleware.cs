using Application.Exceptions;
using Application.Wrappers;
using System.Net;
using System.Text.Json;

namespace WebAPI.Middlewares
{
	public class ErrorHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ErrorHandlerMiddleware> _logger;

		public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
		{
			_next = next;
			_logger = logger;
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
				_logger.LogError(error.Message);
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
