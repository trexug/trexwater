using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TrexWater.Common;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater.Watering
{
	public class WaterController : IWaterController
	{
		public const bool PIN_ON = false;
		public const bool PIN_OFF = true;
		public IGpioPin Pin { get; }
		private DateTime TurnedOnTime { get; set; }
		public bool IsOn { get; private set; }
		public double LitersPerSecond { get; }
		private ITimeProvider TimeProvider { get; }

		public WaterController(IGpioPin pin, double litersPerSecond, ITimeProvider timeProvider)
		{
			TimeProvider = timeProvider;

			Pin = pin;
			LitersPerSecond = litersPerSecond;
			TurnedOnTime = default;
			IsOn = false;
			TurnedOnTime = default;
		}

		public void TurnOn()
		{
			if (IsOn)
			{
				throw new InvalidOperationException($"{nameof(WaterController)} is already on.");
			}

			Pin.Write(PIN_ON);
			IsOn = true;
			TurnedOnTime = TimeProvider.Now;
		}

		public WateringSession TurnOff()
		{
			if (!IsOn)
			{
				throw new InvalidOperationException($"{nameof(WaterController)} is already off.");
			}
			Pin.Write(PIN_OFF);
			IsOn = false;
			DateTime now = TimeProvider.Now;
			TimeSpan span = now - TurnedOnTime;
			return new WateringSession(TurnedOnTime, now, span.TotalSeconds * LitersPerSecond);
		}
	}
}
