using Microsoft.Extensions.Logging;
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
		private ILoggerFactory LoggerFactory { get; }
		public WaterControllerFactory(ILoggerFactory loggerFactory, ITimeProvider timeProvider)
		{
			LoggerFactory = loggerFactory;
			TimeProvider = timeProvider;
		}

		public IWaterController CreateWaterController(string id, IGpioPin pin, double litersPerSecond)
		{
			return new WaterController(id, LoggerFactory, pin, litersPerSecond, TimeProvider);
		}
	}
}
