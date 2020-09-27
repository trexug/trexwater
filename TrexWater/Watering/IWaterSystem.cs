using System;

namespace TrexWater.Watering
{
	public interface IWaterSystem : IDisposable
	{
		IWaterController this[string id] { get; }

		bool IsInitialized { get; }

		bool ContainsId(string id);
		void Initialize();
		bool TryGet(string id, out IWaterController waterController);
	}
}