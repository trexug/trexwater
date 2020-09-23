using System;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater
{
	class Program
	{
		static void Main(string[] args)
		{
			DD();
		}

		private static void DD()
		{
			WaterController controller = new WaterController(Pi.Gpio[BcmPin.Gpio26], 0.01);

			while (true)
			{
				controller.TurnOn();
				Thread.Sleep(1000);
				controller.TurnOff();
			}
		}
	}
}
