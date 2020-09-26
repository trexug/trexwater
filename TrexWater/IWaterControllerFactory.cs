using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater
{
	public interface IWaterControllerFactory
	{
		IWaterController CreateWaterController(IGpioPin pin, double litersPerSecond);
	}
}