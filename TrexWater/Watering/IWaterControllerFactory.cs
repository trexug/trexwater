using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater.Watering
{
	public interface IWaterControllerFactory
	{
		public IWaterController CreateWaterController(string id, IGpioPin pin, double litersPerSecond);
	}
}