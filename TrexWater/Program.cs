using System;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace TrexWater
{
	class Program
	{
		static void Main(string[] args)
		{
			Pi.Init<BootstrapWiringPi>();
			DD();
		}

		private static void DD()
		{
			var pin = Pi.Gpio[BcmPin.Gpio26];
			pin.PinMode = GpioPinDriveMode.Output;
			WaterController controller = new WaterController(pin, 0.01);

			while (true)
			{
				controller.TurnOn();
				Thread.Sleep(1000);
				controller.TurnOff();
			}
		}
	}
}
