using Microsoft.Extensions.Logging;
using System;
using TrexWater.Common;
using Unosquare.RaspberryIO.Abstractions;

namespace TrexWater.Watering
{
	public class WaterController : IWaterController
	{
		public const bool PIN_ON = false;
		public const bool PIN_OFF = true;

		public event EventHandler<WaterControllerOnEventArgs> FlowOn;
		public event EventHandler<WaterControllerOffEventArgs> FlowOff;
		public string Id { get; }
		public IGpioPin Pin { get; }
		private DateTime TurnedOnTime { get; set; }
		public bool IsOn { get; private set; }
		public double LitersPerSecond { get; }
		private ITimeProvider TimeProvider { get; }
		private ILogger<WaterController> Logger { get; }
		public WaterController(string id, ILoggerFactory loggerFactory, IGpioPin pin, double litersPerSecond, ITimeProvider timeProvider)
		{
			Id = id;
			Logger = loggerFactory.CreateLogger<WaterController>();

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
			Logger.LogTrace("On written to bcm pin: '{0}'", Pin.BcmPin);
			TurnedOnTime = TimeProvider.Now;
			FlowOn?.Invoke(this, new WaterControllerOnEventArgs());
		}

		public WateringSession TurnOff()
		{
			if (!IsOn)
			{
				throw new InvalidOperationException($"{nameof(WaterController)} is already off.");
			}
			Pin.Write(PIN_OFF);
			IsOn = false;
			Logger.LogTrace("Off written to bcm pin: '{0}'", Pin.BcmPin);
			DateTime now = TimeProvider.Now;
			TimeSpan span = now - TurnedOnTime;
			Logger.LogTrace("Pin: '{0}' was on for {1:0.00} seconds", Pin.BcmPin, span.TotalSeconds);
			WateringSession session = new WateringSession(TurnedOnTime, now, span.TotalSeconds * LitersPerSecond);
			FlowOff?.Invoke(this, new WaterControllerOffEventArgs(session));
			return session;
		}
	}
}
