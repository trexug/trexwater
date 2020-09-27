using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TrexWater.Messaging
{
	public class TrexWaterMqttClient
	{
		public const string ON_PAYLOAD = "ON";
		public const string OFF_PAYLOAD = "OFF";
		public const string ONLINE_PAYLOAD = "ONLINE";
		public const string OFFLINE_PAYLOAD = "OFFLINE";

		public const string STATUS_TOPIC = "tele";
		public const string RESPONSE_TOPIC = "stat";
		public const string COMMAND_TOPIC = "cmnd";
		private IManagedMqttClient Client { get; }
		private ManagedMqttClientOptions ClientOptions { get; }
		private MqttOptions Options { get; }
		public string GetFlowPostfix(string waterControllerId) => $"{Options.LocalDeviceName}/{waterControllerId}/FLOW"; // Eg. wdev/bas01/FLOW
		public string GetFlowResponseTopic(string waterControllerId) => $"{RESPONSE_TOPIC}/{GetFlowPostfix(waterControllerId)}"; // Eg. stat/wdev/bas01/FLOW
		public string GetResultResponseTopic(string waterControllerId) => $"{RESPONSE_TOPIC}/{Options.LocalDeviceName}/{waterControllerId}/RESULT"; // Eg. stat/wdev/bas01/RESULT
		public string OnlineStatusTopic => $"{STATUS_TOPIC}/{Options.LocalDeviceName}/LWT"; // Eg. tele/wdev/LWT
		public string GetFlowStatusTopic(string waterControllerId) => $"{STATUS_TOPIC}/{GetFlowPostfix(waterControllerId)}"; // Eg. tele/wdev/bas01/FLOW
		public string CommandTopicSubscription => $"{COMMAND_TOPIC}/{Options.LocalDeviceName}/#"; // Eg. cmnd/wdev/#
		public TrexWaterMqttClient(MqttOptions options)
		{
			Options = options;
			ClientOptions = new ManagedMqttClientOptionsBuilder()
				.WithAutoReconnectDelay(TimeSpan.FromMilliseconds(Options.ReconnectTimeoutMiliseconds))
				.WithClientOptions
				(
					ConfigureClientOptions(new MqttClientOptionsBuilder(), Options).Build()
				).Build();

			Client = new MqttFactory().CreateManagedMqttClient();
			
		}

		private MqttClientOptionsBuilder ConfigureClientOptions(MqttClientOptionsBuilder builder, MqttOptions options)
		{
			builder
				.WithClientId(options.ClientName)
				.WithTcpServer(options.Host, options.Port)
				.WithWillMessage(GetLastWill());
			if (options.UseTls)
			{
				builder = builder.WithTls();
			}
			if (options.HasCredentials)
			{
				builder = builder.WithCredentials(options.Username, options.Password);
			}
			return builder;
		}

		private MqttApplicationMessage GetLastWill()
		{
			return CreateMessage(OnlineStatusTopic, OFFLINE_PAYLOAD, true);
		}

		public Task StartAsync()
		{
			return StartAsync(CancellationToken.None);
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await Client.SubscribeAsync(CommandTopicSubscription);
			await Client.StartAsync(ClientOptions);
			await SendMessageAsync(OnlineStatusTopic, ONLINE_PAYLOAD, true, cancellationToken);
		}
		public Task SendCommandResponseAsync(string waterControllerId, string result, bool isOn)
		{
			return SendCommandResponseAsync(waterControllerId, result, isOn, CancellationToken.None);
		}

		public Task SendCommandResponseAsync(string waterControllerId, string result, bool isOn, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.WhenAll
			(
				SendMessageAsync(GetResultResponseTopic(waterControllerId), result, false, cancellationToken),
				SendMessageAsync(GetFlowResponseTopic(waterControllerId), isOn ? ON_PAYLOAD : OFF_PAYLOAD, false, cancellationToken)
			);
		}

		public Task SendFlowStatusAsync(string waterControllerId, bool on)
		{
			return SendFlowStatusAsync(waterControllerId, on, CancellationToken.None);
		}
		public Task SendFlowStatusAsync(string waterControllerId, bool isOn, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return SendMessageAsync(GetFlowStatusTopic(waterControllerId), isOn ? ON_PAYLOAD : OFF_PAYLOAD, true, cancellationToken);
		}

		public Task SendOnlineStatusAsync(bool isOnline)
		{
			return SendOnlineStatusAsync(isOnline, CancellationToken.None);
		}
		public Task SendOnlineStatusAsync(bool isOnline, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return SendMessageAsync(OnlineStatusTopic, isOnline ? ONLINE_PAYLOAD : OFFLINE_PAYLOAD, true, cancellationToken);
		}

		private Task SendMessageAsync(string topic, string payload, bool retain, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var message = CreateMessage(topic, payload, retain);
			return Client.PublishAsync(message, cancellationToken);
		}

		private MqttApplicationMessage CreateMessage(string topic, string payload, bool retain)
		{
			return new MqttApplicationMessageBuilder()
				.WithTopic(topic)
				.WithPayload(payload)
				.WithAtLeastOnceQoS()
				.WithRetainFlag(retain)
				.Build();
		}
	}
}
