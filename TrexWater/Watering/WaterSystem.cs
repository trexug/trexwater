using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using TrexWater.Gpio;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater.Watering
{
	public class WaterSystem
	{
		private IWaterControllerFactory WaterControllerFactory { get; }
		private IGpioPinFactory GpioPinFactory { get; }
		private Dictionary<string, IWaterController> IdToWaterController { get; }
		public bool IsInitialized { get; private set; }
		private ILogger<WaterSystem> Logger { get; }
		public WaterSystem(IWaterControllerFactory waterControllerFactory, IGpioPinFactory gpioPinFactory, ILoggerFactory loggerFactory)
		{
			Logger = loggerFactory.CreateLogger<WaterSystem>();

			WaterControllerFactory = waterControllerFactory;
			GpioPinFactory = gpioPinFactory;

			IdToWaterController = new Dictionary<string, IWaterController>();
			IsInitialized = false;
		}

		public void Initialize()
		{
			if (IsInitialized)
			{
				throw new InvalidOperationException($"{nameof(WaterSystem)} is already initialized");
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
				pin.Write(WaterController.PIN_OFF);
				IWaterController waterController = WaterControllerFactory.CreateWaterController(pin, config.LitersPerSecond);

				IdToWaterController.Add(config.Id, waterController);
			}

			IsInitialized = true;
			Logger.LogInformation("Application initialized");
		}

		public IWaterController this[string id] => IdToWaterController[id];

		public void Run()
		{
			if (!IsInitialized)
			{
				throw new InvalidOperationException($"{nameof(WaterSystem)} is not initialized");
			}
			Logger.LogInformation("Application starting..");
			DisableWaterControllers();

			Logger.LogInformation("Application running");
			try
			{
				var c = IdToWaterController["B01"];
				for (int i = 0;  i < 10; ++i)
				{
					c.TurnOn();
					Logger.LogTrace("Going to sleep..");
					Thread.Sleep(2000);
					c.TurnOff();
					Logger.LogTrace("Going to sleep..");
					Thread.Sleep(500);
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
