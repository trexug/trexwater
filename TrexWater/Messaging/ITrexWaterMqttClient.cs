using System.Threading;
using System.Threading.Tasks;

namespace TrexWater.Messaging
{
	public interface ITrexWaterMqttClient
	{
		string CommandTopicSubscription { get; }
		string OnlineStatusTopic { get; }

		string GetFlowPostfix(string waterControllerId);
		string GetFlowResponseTopic(string waterControllerId);
		string GetFlowStatusTopic(string waterControllerId);
		string GetResultResponseTopic(string waterControllerId);
		Task SendCommandResponseAsync(string waterControllerId, string result, bool isOn);
		Task SendCommandResponseAsync(string waterControllerId, string result, bool isOn, CancellationToken cancellationToken);
		Task SendFlowStatusAsync(string waterControllerId, bool on);
		Task SendFlowStatusAsync(string waterControllerId, bool isOn, CancellationToken cancellationToken);
		Task SendOnlineStatusAsync(bool isOnline);
		Task SendOnlineStatusAsync(bool isOnline, CancellationToken cancellationToken);
		Task StartAsync();
		Task StartAsync(CancellationToken cancellationToken);
	}
}