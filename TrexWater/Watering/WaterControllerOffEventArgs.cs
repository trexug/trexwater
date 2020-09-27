using System;

namespace TrexWater.Watering
{
	public class WaterControllerOffEventArgs : EventArgs
	{
		public WateringSession Session { get; }
		public WaterControllerOffEventArgs(WateringSession session)
		{
			Session = session;
		}
	}
}
