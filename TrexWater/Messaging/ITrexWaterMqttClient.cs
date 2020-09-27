using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace TrexWater.Messaging
{
	public interface ITrexWaterMqttClient : IDisposable
	{
		Task StartAsync();
		Task StartAsync(CancellationToken cancellationToken);
	}
}