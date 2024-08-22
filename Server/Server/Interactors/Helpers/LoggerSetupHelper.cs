using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;


namespace Interactors.Helpers
{
	public static class LoggerSetupHelper
	{
		public static void ConfigureLogging()
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
				.Enrich.FromLogContext()
				.Enrich.WithClientIp()
				.Enrich.WithExceptionData()
				.Enrich.WithMemoryUsage()
				.Enrich.WithProcessName()
				.Enrich.WithThreadName()
				.Enrich.WithDemystifiedStackTraces()
				.Enrich.WithRequestUserId()
				.WriteTo.Console(theme: SystemConsoleTheme.Colored, restrictedToMinimumLevel: LogEventLevel.Information)
				.WriteTo.Console(theme: SystemConsoleTheme.Colored, restrictedToMinimumLevel: LogEventLevel.Error)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[VERBOSE]_Message_Exchange_Log_.log",
					rollingInterval: RollingInterval.Day,
					rollOnFileSizeLimit: true,
					retainedFileCountLimit: 365,
					shared: true,
					restrictedToMinimumLevel: LogEventLevel.Verbose)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[ERROR]_Message_Exchange_Log_.log",
					rollingInterval: RollingInterval.Day,
					rollOnFileSizeLimit: true,
					retainedFileCountLimit: 365,
					shared: true,
					restrictedToMinimumLevel: LogEventLevel.Error)
				.WriteTo.File(AppContext.BaseDirectory + @"\Log\[INFO]_Message_Exchange_Log_.log",
					rollingInterval: RollingInterval.Day,
					rollOnFileSizeLimit: true,
					retainedFileCountLimit: 365,
					shared: true,
					restrictedToMinimumLevel: LogEventLevel.Information)
				.CreateLogger();
		}
	}
}
