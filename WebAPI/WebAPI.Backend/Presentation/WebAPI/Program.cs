using Application;
using Application.Mappings;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Persistance;
using Persistance.DbContexts;
using Prometheus;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Shared;
using System.Reflection;
using WebAPI.Extensions;

try
{
	Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
	.Enrich.FromLogContext()
	.Enrich.WithExceptionData()
	.Enrich.WithThreadName()
	.Enrich.WithDemystifiedStackTraces()
	.WriteTo.Console(theme: SystemConsoleTheme.Colored, restrictedToMinimumLevel: LogEventLevel.Information)
	.WriteTo.Console(theme: SystemConsoleTheme.Colored, restrictedToMinimumLevel: LogEventLevel.Error)
	.WriteTo.File(AppContext.BaseDirectory + @"\Log\[VERBOSE]_Log_.log",
			rollingInterval: RollingInterval.Day,
			rollOnFileSizeLimit: true,
			retainedFileCountLimit: 365,
			shared: true,
			restrictedToMinimumLevel: LogEventLevel.Verbose)
	.WriteTo.File(AppContext.BaseDirectory + @"\Log\[ERROR]_Log_.log",
			rollingInterval: RollingInterval.Day,
			rollOnFileSizeLimit: true,
			retainedFileCountLimit: 365,
			shared: true,
			restrictedToMinimumLevel: LogEventLevel.Error)
	.WriteTo.File(AppContext.BaseDirectory + @"\Log\[INFO]_Log_.log",
			rollingInterval: RollingInterval.Day,
			rollOnFileSizeLimit: true,
			retainedFileCountLimit: 365,
			shared: true,
			restrictedToMinimumLevel: LogEventLevel.Information)
	.CreateLogger();

	
	Log.Information("Application startup services registration");

	var builder = Host.CreateDefaultBuilder().UseSerilog(Log.Logger)
		.ConfigureWebHostDefaults(webBuilder =>
		{
			webBuilder.ConfigureServices((context, services) =>
			{
				services.AddMemoryCache();
				services.AddAplication();
				services.AddPersistance(context.Configuration);
				services.AddShared(context.Configuration);
				services.AddAutoMapper(config =>
				{
					config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
				});
				services.AddSwaggerExtension();
				services.AddControllersExtension();
				services.AddCorsExtension();
				services.AddOpenTelemetry()
						.WithMetrics(metricBuilder =>
						{
							metricBuilder
								.AddConsoleExporter()
								.AddAspNetCoreInstrumentation()
								.AddHttpClientInstrumentation()
								.AddRuntimeInstrumentation()
								.AddPrometheusExporter()
								.AddMeter("ServerComponents","ClientComponents", "WebAPIComponents");
						})
						.WithTracing(traceBuilder =>
						{
							traceBuilder
								.AddSource("webapi")
								.SetErrorStatusOnException(true)
								.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("webapi"))
								.AddAspNetCoreInstrumentation(opts =>
								{
									opts.Filter = req => !req.Request.Path.Value.Contains("/metrics");
								})
								.AddHttpClientInstrumentation()
								.AddEntityFrameworkCoreInstrumentation();
						});
				

				services.AddHealthChecks();
				services.AddJWTAuthentication(context.Configuration);
				services.AddAuthorizationPolicies(context.Configuration);
				services.AddMvcCore()
				.AddApiExplorer();
				services.AddMvcCore();
			});

			webBuilder.Configure(app =>
			{
				Log.Information("Application startup middleware registration");

				var env = app.ApplicationServices.GetService<IHostEnvironment>();

				if (env != null && env.IsDevelopment())
				{
					app.UseDeveloperExceptionPage();

					using (var scope = app.ApplicationServices.CreateScope())
					{
						var dbConnectionContext = scope.ServiceProvider.GetRequiredService<ConnectionDbContext>();
						dbConnectionContext.Database.EnsureCreated();
						var dbClientConfigContext = scope.ServiceProvider.GetRequiredService<ClientConfigDbContext>();
						dbClientConfigContext.Database.EnsureCreated();
						var dbServerConfigContext = scope.ServiceProvider.GetRequiredService<ServerConfigDbContext>();
						dbServerConfigContext.Database.EnsureCreated();
					}
				}
				else
				{
					app.UseExceptionHandler("/Error");
					app.UseHsts();
				}
				
				app.UseSerilogRequestLogging();
				app.UseHttpsRedirection();
				app.UseRouting();
				app.UseCors("AllowAll");
				app.UseAuthentication();
				app.UseAuthorization();
				app.UseSwaggerExtension();
				app.UseErrorHandlingMiddleware();
				app.UseHealthChecks("/health");
				app.UseOpenTelemetryPrometheusScrapingEndpoint();
				app.UseMetricServer();
				app.UseHttpMetrics();
				app.UseEndpoints(endpoints =>
				{
					endpoints.MapControllers();
				});

				Log.Information("Application Starting");
			});
		});
	var app = builder.Build();
	app.Run();

}
catch (Exception ex)
{
	Log.Warning(ex, "An error occurred starting the application");
}
finally
{
	Log.CloseAndFlush();
}

