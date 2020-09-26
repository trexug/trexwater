using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater
{
	public class PiGpioPinFactory : IGpioPinFactory
	{
		public IGpioPin CreatePin(BcmPin id)
		{
			return Pi.Gpio[id];
		}
	}
}
