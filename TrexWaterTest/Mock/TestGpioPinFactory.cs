using TrexWater;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWaterTest.Mock
{
	public class TestGpioPinFactory : IGpioPinFactory
	{
		public IGpioPin CreatePin(BcmPin id)
		{
			return new TestGpioPin(id);
		}
	}
}
