using System;
using System.Collections.Generic;
using System.Text;
using TrexWater.Common;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater.Watering
{
	public class WaterControllerFactory : IWaterControllerFactory
	{
		private ITimeProvider TimeProvider { get; }
		public WaterControllerFactory(ITimeProvider timeProvider)
		{
			TimeProvider = timeProvider;
		}

		public IWaterController CreateWaterController(IGpioPin pin, double litersPerSecond)
		{
			return new WaterController(pin, litersPerSecond, TimeProvider);
		}
	}
}
