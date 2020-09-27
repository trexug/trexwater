using System;
using System.Collections.Generic;

namespace TrexWater.Watering
{
	public interface IWaterSystem : IDisposable, IEnumerable<IWaterController>
	{
		IWaterController this[string id] { get; }

		bool IsInitialized { get; }

		bool ContainsId(string id);
		void Initialize();
		bool TryGet(string id, out IWaterController waterController);
	}
}