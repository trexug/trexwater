using System;
using System.Collections.Generic;
using System.Text;
using TrexWater;
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
			WaterController01 = new WaterControllerContext(Unosquare.RaspberryIO.Abstractions.BcmPin.Gpio00, 0);
			WaterController02 = new WaterControllerContext(Unosquare.RaspberryIO.Abstractions.BcmPin.Gpio00, 0.016);
			WaterController03 = new WaterControllerContext(Unosquare.RaspberryIO.Abstractions.BcmPin.Gpio00, 2);
			WaterController04 = new WaterControllerContext(Unosquare.RaspberryIO.Abstractions.BcmPin.Gpio00, 0.05);
			WaterController05 = new WaterControllerContext(Unosquare.RaspberryIO.Abstractions.BcmPin.Gpio00, 0.12);
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
			Assert.ThrowsAny<InvalidOperationException>(() => WaterController01.WaterController.TurnOff());
		}
		[Fact]
		public void TurnSetGpio()
		{
			WaterController01.WaterController.TurnOn();
			Assert.True(WaterController01.GpioPin.Value);
			WaterController01.WaterController.TurnOff();
			Assert.False(WaterController01.GpioPin.Value);
			WaterController01.WaterController.TurnOn();
			Assert.True(WaterController01.GpioPin.Value);
			WaterController01.WaterController.TurnOff();
			Assert.False(WaterController01.GpioPin.Value);
		}
		[Fact]
		public void CalculatesWaterUsage()
		{

		}
	}
}
