using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater
{
	public class PiGpioPinFactory : IGpioPinFactory
	{
		public IGpioPin CreatePin(BcmPin id, GpioPinDriveMode mode = GpioPinDriveMode.Output)
		{
			var pin = Pi.Gpio[id];
			pin.PinMode = mode;
			return pin;
		}
	}
}
