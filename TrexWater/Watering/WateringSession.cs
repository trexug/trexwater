using System;
using System.Collections.Generic;
using System.Text;

namespace TrexWater.Watering
{
	public class WateringSession
	{
		public int? Id { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public double AmountInLiters { get; set; }

		public WateringSession(DateTime startTime, DateTime endTime, double amountInLiters)
		{
			StartTime = startTime;
			EndTime = endTime;
			AmountInLiters = amountInLiters;
		}
	}
}
