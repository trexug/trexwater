using System;

namespace TrexWater
{
	public interface ITimeProvider
	{
		DateTime Now { get; }
	}
}