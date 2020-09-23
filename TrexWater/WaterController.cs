using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater
{
	public class WaterController
	{
		public IGpioPin Pin { get; }
		private DateTime TurnedOnTime { get; set; }
		public bool IsOn { get; private set; }
		public double LitersPerSecond { get; }

		public WaterController(IGpioPin pin, double litersPerSecond)
		{
			Pin = pin;
			LitersPerSecond = litersPerSecond;
			TurnedOnTime = default;
			IsOn = pin.Value;
		}

		public void TurnOn()
		{
			if (!IsOn)
			{
				throw new InvalidOperationException($"{nameof(WaterController)} is already on.");
			}

			Pin.Write(true);
			IsOn = true;
			TurnedOnTime = DateTime.Now;
		}

		public WateringSession TurnOff()
		{
			if (!IsOn)
			{
				throw new InvalidOperationException($"{nameof(WaterController)} is already off.");
			}

			Pin.Write(false);
			IsOn = false;
			DateTime now = DateTime.Now;
			TimeSpan span = now - TurnedOnTime;
			return new WateringSession(TurnedOnTime, now, span.TotalSeconds * LitersPerSecond);
		}
	}
}
