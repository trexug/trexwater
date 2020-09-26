using System;
using System.Collections.Generic;
using System.Text;
using TrexWater;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWaterTest.Mock
{
	public class WaterControllerContext
	{
		public WaterController WaterController { get; }
		public TestTimeProvider TimeProvider { get; }
		public TestGpioPin GpioPin { get; }
		public WaterControllerContext(BcmPin id, double litersPerSecond)
		{
			TimeProvider = new TestTimeProvider();
			GpioPin = new TestGpioPin(id);
			WaterController = new WaterController(GpioPin, litersPerSecond, TimeProvider);
		}
	}
}
