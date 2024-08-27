using Opc.Ua.Configuration;
using Serilog;

namespace Opc.Ua.Sample
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			ApplicationInstance application = new ApplicationInstance();
			application.ApplicationName = "UA gateway";
			application.ApplicationType = ApplicationType.Client;
			application.ConfigSectionName = "Opc.Ua.Client";		
			try
			{
				if (application.ProcessCommandLine())
				{
					return;
				}

				// load the application configuration.
				application.LoadApplicationConfiguration(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml"), false);

				// check the application certificate.
				application.CheckApplicationInstanceCertificate(false, 0);
			}
			catch (Exception e)
			{
				Log.Error(application.ApplicationName, e);
				return;
			}
		}
	}
}
