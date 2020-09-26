using System;

namespace TrexWater.Common
{
	public interface ITimeProvider
	{
		DateTime Now { get; }
	}
}