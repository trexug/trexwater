using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Unosquare.RaspberryIO;
using Unosquare.WiringPi;

namespace TrexWater
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Program program = new Program();
			program.RunMain();
		}

		public void RunMain()
		{
			var serviceProvider = ConfigureServices(new ServiceCollection());

			var logger = serviceProvider.GetService<ILoggerFactory>()
				.CreateLogger<Program>();
			logger.LogDebug("Service setup complete");

			var application = serviceProvider.GetService<IApplication>();
			application.Initialize();
			application.Run();
		}

		protected virtual IServiceProvider ConfigureServices(IServiceCollection serviceCollection)
		{
			Pi.Init<BootstrapWiringPi>();
			return serviceCollection
				.AddLogging(lb => lb.AddConsole())
				.AddSingleton<ITimeProvider, TimeProvider>()
				.AddSingleton<IGpioPinFactory, PiGpioPinFactory>()
				.AddSingleton<IWaterControllerFactory, WaterControllerFactory>()
				.AddSingleton<IApplication, Application>()
				.BuildServiceProvider();
		}
	}
}
