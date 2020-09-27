namespace TrexWater.Watering
{
	public interface IWaterSystem
	{
		IWaterController this[string id] { get; }

		bool IsInitialized { get; }

		bool ContainsId(string id);
		void Initialize();
		bool TryGet(string id, out IWaterController waterController);
	}
}