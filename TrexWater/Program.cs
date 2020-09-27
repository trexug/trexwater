using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using TrexWater.Common;
using TrexWater.Gpio;
using TrexWater.Messaging;
using TrexWater.Watering;
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

			var client = serviceProvider.GetService<ITrexWaterMqttClient>();
			client.StartAsync().Wait();
			Console.ReadLine();
		}

		protected virtual IServiceProvider ConfigureServices(IServiceCollection serviceCollection)
		{
			//Pi.Init<BootstrapWiringPi>();

			IConfigurationRoot configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.Build();
			
			serviceCollection
				.AddLogging
				(
					lb => lb
						.AddConsole()
						.SetMinimumLevel(LogLevel.Trace)
				)
				.AddSingleton<ITimeProvider, TimeProvider>()
				.AddSingleton<IGpioPinFactory, PiGpioPinFactory>()
				.AddSingleton<IWaterControllerFactory, WaterControllerFactory>()
				.AddSingleton<IWaterSystem, WaterSystem>()
				.AddSingleton<ITrexWaterMqttClient, TrexWaterMqttClient>();

			serviceCollection.AddOptions<MqttOptions>()
				.Bind(configuration.GetSection(MqttOptions.MQTT));

			return serviceCollection.BuildServiceProvider();
		}
	}
}
