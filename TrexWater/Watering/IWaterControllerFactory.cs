﻿using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater.Watering
{
	public interface IWaterControllerFactory
	{
		IWaterController CreateWaterController(IGpioPin pin, double litersPerSecond);
	}
}