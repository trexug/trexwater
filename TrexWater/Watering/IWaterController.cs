using System;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater.Watering
{
	public interface IWaterController
	{
		public event EventHandler<WaterControllerOnEventArgs> FlowOn;
		public event EventHandler<WaterControllerOffEventArgs> FlowOff;
		bool IsOn { get; }
		double LitersPerSecond { get; }
		IGpioPin Pin { get; }
		WateringSession TurnOff();
		void TurnOn();
	}
}