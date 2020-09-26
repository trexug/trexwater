using System;
using System.Collections.Generic;
using System.Text;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWaterTest.Mock
{
	public class TestGpioPin : IGpioPin
	{
		public TestGpioPin(BcmPin id)
		{
			BcmPin = id;
			PinMode = GpioPinDriveMode.Input;
		}
		public BcmPin BcmPin { get; }

		public int BcmPinNumber => (int)BcmPin;

		public int PhysicalPinNumber => throw new NotImplementedException();

		public GpioHeader Header => throw new NotImplementedException();

		public GpioPinDriveMode PinMode { get; set; }
		public GpioPinResistorPullMode InputPullMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool Value
		{
			get => throw new NotImplementedException(); set => throw new NotImplementedException();
		}

		public bool Read()
		{
			throw new NotImplementedException();
		}

		public void RegisterInterruptCallback(EdgeDetection edgeDetection, Action callback)
		{
			throw new NotImplementedException();
		}

		public void RegisterInterruptCallback(EdgeDetection edgeDetection, Action<int, int, uint> callback)
		{
			throw new NotImplementedException();
		}

		public void RemoveInterruptCallback(EdgeDetection edgeDetection, Action callback)
		{
			throw new NotImplementedException();
		}

		public void RemoveInterruptCallback(EdgeDetection edgeDetection, Action<int, int, uint> callback)
		{
			throw new NotImplementedException();
		}

		public bool WaitForValue(GpioPinValue status, int timeOutMillisecond)
		{
			throw new NotImplementedException();
		}

		public void Write(bool value)
		{
			Value = value;
		}

		public void Write(GpioPinValue value)
		{
			Value = value == GpioPinValue.High;
		}
	}
}
