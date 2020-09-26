using System;
using TrexWater;

namespace TrexWaterTest.Mock
{
	public class TestTimeProvider : ITimeProvider
	{
		public DateTime Now { get; set; }
	}
}
