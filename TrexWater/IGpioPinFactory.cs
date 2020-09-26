using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater
{
	public interface IGpioPinFactory
	{
		IGpioPin CreatePin(BcmPin id);
	}
}