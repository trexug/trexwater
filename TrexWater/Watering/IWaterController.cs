using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater.Watering
{
	public interface IWaterController
	{
		bool IsOn { get; }
		double LitersPerSecond { get; }
		IGpioPin Pin { get; }
		WateringSession TurnOff();
		void TurnOn();
	}
}