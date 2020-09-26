using TrexWater;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWaterTest.Mock
{
	public class TestGpioPinFactory : IGpioPinFactory
	{
		public IGpioPin CreatePin(BcmPin id, GpioPinDriveMode mode = GpioPinDriveMode.Output)
		{
			return new TestGpioPin(id);
		}
	}
}
