using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TrexWater;
using TrexWater.Watering;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWaterTest.Mock
{
	public class WaterControllerContext
	{
		public WaterController WaterController { get; }
		public TestTimeProvider TimeProvider { get; }
		public TestGpioPin GpioPin { get; }
		public WaterControllerContext(string id, BcmPin pin, double litersPerSecond)
		{
			TimeProvider = new TestTimeProvider();
			GpioPin = new TestGpioPin(pin);
			GpioPin.PinMode = GpioPinDriveMode.Output;
			GpioPin.Write(WaterController.PIN_OFF);
			WaterController = new WaterController(id, new LoggerFactory(), GpioPin, litersPerSecond, TimeProvider);
		}
	}
}
