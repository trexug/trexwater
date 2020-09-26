using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater
{
	public class Application : IApplication
	{
		private IWaterControllerFactory WaterControllerFactory { get; }
		private IGpioPinFactory GpioPinFactory { get; }
		private Dictionary<string, IWaterController> IdToWaterController { get; }
		public bool IsInitialized { get; private set; }
		private ILogger<Application> Logger { get; }
		public Application(IWaterControllerFactory waterControllerFactory, IGpioPinFactory gpioPinFactory, ILoggerFactory loggerFactory)
		{
			Logger = loggerFactory.CreateLogger<Application>();

			WaterControllerFactory = waterControllerFactory;
			GpioPinFactory = gpioPinFactory;

			IdToWaterController = new Dictionary<string, IWaterController>();
			IsInitialized = false;
		}

		public void Initialize()
		{
			if (IsInitialized)
			{
				throw new InvalidOperationException($"{nameof(Application)} is already initialized");
			}
			Logger.LogTrace("Initializing..");
			var waterControllerConfigs = new[]
			{
				new { Id = "B01", PinId = BcmPin.Gpio26, LitersPerSecond = 0.01 },
				new { Id = "B02", PinId = BcmPin.Gpio20, LitersPerSecond = 0.01 },
				new { Id = "B03", PinId = BcmPin.Gpio21, LitersPerSecond = 0.01 },
			};

			foreach (var config in waterControllerConfigs)
			{
				IGpioPin pin = GpioPinFactory.CreatePin(config.PinId);
				IWaterController waterController = WaterControllerFactory.CreateWaterController(pin, config.LitersPerSecond);

				IdToWaterController.Add(config.Id, waterController);
			}

			IsInitialized = true;
			Logger.LogInformation("Application initialized");
		}
		public void Run()
		{
			if (!IsInitialized)
			{
				throw new InvalidOperationException($"{nameof(Application)} is not initialized");
			}
			Logger.LogInformation("Application starting..");
			DisableWaterControllers();

			Logger.LogInformation("Application running");
			try
			{
				var c = IdToWaterController["B01"];
				while (true)
				{
					c.TurnOn();
					Logger.LogTrace("Going to sleep..");
					Thread.Sleep(1500);
					c.TurnOff();
					Logger.LogTrace("Going to sleep..");
					Thread.Sleep(1500);
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Application encountered an error.");
			}

			Logger.LogInformation("Application stopped.");
		}

		private void DisableWaterControllers()
		{
			Logger.LogTrace("Disabling water controllers..");
			foreach (var waterController in IdToWaterController.Values)
			{
				if (waterController.IsOn)
				{
					waterController.TurnOff();
				}
			}
		}
	}
}
