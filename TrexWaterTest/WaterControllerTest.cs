using System;
using TrexWater;
using TrexWater.Watering;
using TrexWaterTest.Mock;
using Xunit;

namespace TrexWaterTest
{
	public class WaterControllerTest
	{
		private WaterControllerContext WaterController01 { get; }
		private WaterControllerContext WaterController02 { get; }
		private WaterControllerContext WaterController03 { get; }
		private WaterControllerContext WaterController04 { get; }
		private WaterControllerContext WaterController05 { get; }
		public WaterControllerTest()
		{
			WaterController01 = new WaterControllerContext("T01", Unosquare.RaspberryIO.Abstractions.BcmPin.Gpio00, 0);
			WaterController02 = new WaterControllerContext("T02", Unosquare.RaspberryIO.Abstractions.BcmPin.Gpio26, 0.016);
			WaterController03 = new WaterControllerContext("T03", Unosquare.RaspberryIO.Abstractions.BcmPin.Gpio20, 2);
			WaterController04 = new WaterControllerContext("T04", Unosquare.RaspberryIO.Abstractions.BcmPin.Gpio21, 0.05);
			WaterController05 = new WaterControllerContext("T05", Unosquare.RaspberryIO.Abstractions.BcmPin.Gpio26, 0.12);
		}
		[Fact]
		public void TurnOnTwiceFails()
		{
			WaterController01.WaterController.TurnOn();
			Assert.ThrowsAny<InvalidOperationException>(() => WaterController01.WaterController.TurnOn());
		}
		[Fact]
		public void TurnOffTwiceFails()
		{
			WaterController01.WaterController.TurnOn();
			WaterController01.WaterController.TurnOff();
			Assert.ThrowsAny<InvalidOperationException>(() => WaterController01.WaterController.TurnOff());
		}
		[Fact]
		public void TurnSetGpio()
		{
			WaterController01.WaterController.TurnOn();
			Assert.True(WaterController01.GpioPin.OutputValue == WaterController.PIN_ON);
			WaterController01.WaterController.TurnOff();
			Assert.True(WaterController01.GpioPin.OutputValue == WaterController.PIN_OFF);
			WaterController01.WaterController.TurnOn();
			Assert.True(WaterController01.GpioPin.OutputValue == WaterController.PIN_ON);
			WaterController01.WaterController.TurnOff();
			Assert.True(WaterController01.GpioPin.OutputValue == WaterController.PIN_OFF);
		}
	}
}
