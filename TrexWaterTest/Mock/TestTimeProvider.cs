using System;
using TrexWater;
using TrexWater.Common;

namespace TrexWaterTest.Mock
{
	public class TestTimeProvider : ITimeProvider
	{
		public DateTime Now { get; set; }
	}
}
