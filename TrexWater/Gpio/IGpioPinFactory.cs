using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater.Gpio
{
	public interface IGpioPinFactory
	{
		IGpioPin CreatePin(BcmPin id, GpioPinDriveMode mode = GpioPinDriveMode.Output);
	}
}