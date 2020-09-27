using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TrexWater.Watering;

namespace TrexWater.Messaging
{
	public class TrexWaterMqttClient : ITrexWaterMqttClient
	{
		private const string ON_PAYLOAD = "ON";
		private const string OFF_PAYLOAD = "OFF";
		private const string ONLINE_PAYLOAD = "ONLINE";
		private const string OFFLINE_PAYLOAD = "OFFLINE";
		private const string COMMAND_UNKNOWN_PAYLOAD = "{\"COMMAND\": \"UNKNOWN\"}";
		private const string COMMAND_FLOW_ON_PAYLOAD = "{\"FLOW\": \"ON\"}";
		private const string COMMAND_FLOW_OFF_PAYLOAD = "{\"FLOW\": \"OFF\"}";

		private const string APPLICATION = "trew";
		private const string STATUS_TOPIC = "tele";
		private const string RESPONSE_TOPIC = "stat";
		private const string COMMAND_TOPIC = "cmnd";
		private const string FLOW_TOPIC = "FLOW";

		private const string WATER_CONTROLLER_ID_REGEX_GROUP = "WATER_CONTROLLER_ID";
		private const string COMMAND_NAME_REGEX_GROUP = "COMMAND_NAME";
		private ILogger<TrexWaterMqttClient> Logger { get; }
		private IWaterSystem WaterSystem { get; }
		private IManagedMqttClient Client { get; }
		private ManagedMqttClientOptions ClientOptions { get; }
		private MqttOptions Options { get; }
		private string LocalDeviceWithApplication => $"{Options.LocalDeviceName}/{APPLICATION}";
		private string GetFlowPostfix(string waterControllerId) => $"{LocalDeviceWithApplication}/{waterControllerId}/{FLOW_TOPIC}"; // Eg. wdev/trew/bas01/FLOW
		private string GetFlowResponseTopic(string waterControllerId) => $"{RESPONSE_TOPIC}/{GetFlowPostfix(waterControllerId)}"; // Eg. stat/wdev/trew/bas01/FLOW
		private string GetResultResponseTopic(string waterControllerId) => $"{RESPONSE_TOPIC}/{LocalDeviceWithApplication}/{waterControllerId}/RESULT"; // Eg. stat/wdev/bas01/RESULT
		private string OnlineStatusTopic => $"{STATUS_TOPIC}/{LocalDeviceWithApplication}/LWT"; // Eg. tele/wdev/trew/LWT
		private string GetFlowStatusTopic(string waterControllerId) => $"{STATUS_TOPIC}/{GetFlowPostfix(waterControllerId)}"; // Eg. tele/wdev/trew/bas01/FLOW
		private string CommandTopicSubscription => $"{COMMAND_TOPIC}/{LocalDeviceWithApplication}/#"; // Eg. cmnd/wdev/trew/#
		private string CommandTopicRegexPattern => $"^{COMMAND_TOPIC}/{LocalDeviceWithApplication}/(?<{WATER_CONTROLLER_ID_REGEX_GROUP}>[^/]+)/(?<{COMMAND_NAME_REGEX_GROUP}>[^/]+)$"; // Eg. cmnd/wdev/trew/#
		private Regex CommandTopicRegex { get; }
		public TrexWaterMqttClient(ILoggerFactory loggerFactory, IWaterSystem waterSystem, IOptions<MqttOptions> options)
		{
			Logger = loggerFactory.CreateLogger<TrexWaterMqttClient>();
			WaterSystem = waterSystem;
			Options = options.Value;
			ClientOptions = new ManagedMqttClientOptionsBuilder()
				.WithAutoReconnectDelay(TimeSpan.FromMilliseconds(Options.ReconnectTimeoutMiliseconds))
				.WithClientOptions
				(
					ConfigureClientOptions(new MqttClientOptionsBuilder(), Options).Build()
				).Build();

			Client = new MqttFactory().CreateManagedMqttClient();
			Client.UseApplicationMessageReceivedHandler(HandleMessageReceived);
			Client.UseConnectedHandler(HandleConnected);
			Client.UseDisconnectedHandler(HandleDisconnected);

			CommandTopicRegex = new Regex(CommandTopicRegexPattern, RegexOptions.Compiled);
		}

		private Task HandleMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs)
		{
			return HandleMessageReceived(eventArgs.ApplicationMessage);
		}

		private Task HandleMessageReceived(MqttApplicationMessage message)
		{
			Logger.LogTrace("Message received");
			Match match = CommandTopicRegex.Match(message.Topic);
			if (match.Success)
			{
				string waterControllerId = match.Groups[WATER_CONTROLLER_ID_REGEX_GROUP].Value;
				string command = match.Groups[COMMAND_NAME_REGEX_GROUP].Value;
				string payload = message.ConvertPayloadToString();
				if (!WaterSystem.TryGet(waterControllerId, out IWaterController waterController))
				{
					Logger.LogWarning($"Water controller: '{waterControllerId}' not found");
					return Task.CompletedTask;
				}
				switch (command)
				{
					case FLOW_TOPIC:
						break;
					default:
						Logger.LogWarning($"Unknown command topic '{command}'");
						return SendCommandResponseAsync(waterControllerId, COMMAND_UNKNOWN_PAYLOAD);
				}
				switch (payload)
				{
					case ON_PAYLOAD:
						if (!waterController.IsOn)
						{
							waterController.TurnOn();
						}
						else
						{
							Logger.LogTrace($"Water controller: '{waterControllerId}' is already on");
						}
						return SendCommandResponseAsync(waterControllerId, COMMAND_FLOW_ON_PAYLOAD);
					case OFF_PAYLOAD:
						if (waterController.IsOn)
						{
							waterController.TurnOff();
						}
						else
						{
							Logger.LogTrace($"Water controller: '{waterControllerId}' is already off");
						}
						return SendCommandResponseAsync(waterControllerId, COMMAND_FLOW_OFF_PAYLOAD);
					default:
						Logger.LogWarning($"Unknown payload '{payload}'");
						return Task.CompletedTask;
				}
			}
			else
			{
				Logger.LogTrace("Topic does not match expectation");
			}
			return Task.CompletedTask;
		}

		private void HandleConnected(MqttClientConnectedEventArgs eventArgs)
		{
			Logger.LogInformation($"Connected to {Options.Host}");
		}

		private void HandleDisconnected(MqttClientDisconnectedEventArgs eventArgs)
		{
			Logger.LogWarning("Disconnected");
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
			await SendOnlineStatusAsync(true);
			foreach (var waterController in WaterSystem)
			{
				waterController.FlowOn += WaterControllerFlowOn;
				waterController.FlowOff += WaterControllerFlowOff;
			}
		}

		private async void WaterControllerFlowOff(object sender, WaterControllerOffEventArgs e)
		{
			if (sender is WaterController controller)
			{
				await SendFlowStatusAsync(controller.Id, false);
			}
		}

		private async void WaterControllerFlowOn(object sender, WaterControllerOnEventArgs e)
		{
			if (sender is WaterController controller)
			{
				await SendFlowStatusAsync(controller.Id, true);
			}
		}

		private Task SendCommandResponseAsync(string waterControllerId, string result)
		{
			return SendCommandResponseAsync(waterControllerId, result, CancellationToken.None);
		}

		private Task SendCommandResponseAsync(string waterControllerId, string result, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return SendMessageAsync(GetResultResponseTopic(waterControllerId), result, false, cancellationToken);
		}

		private Task SendFlowStatusAsync(string waterControllerId, bool on)
		{
			return SendFlowStatusAsync(waterControllerId, on, CancellationToken.None);
		}
		private Task SendFlowStatusAsync(string waterControllerId, bool isOn, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return SendMessageAsync(GetFlowStatusTopic(waterControllerId), isOn ? ON_PAYLOAD : OFF_PAYLOAD, true, cancellationToken);
		}

		private Task SendOnlineStatusAsync(bool isOnline)
		{
			return SendOnlineStatusAsync(isOnline, CancellationToken.None);
		}
		private Task SendOnlineStatusAsync(bool isOnline, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return SendMessageAsync(OnlineStatusTopic, isOnline ? ONLINE_PAYLOAD : OFFLINE_PAYLOAD, true, cancellationToken);
		}

		private Task SendMessageAsync(string topic, string payload, bool retain, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var message = CreateMessage(topic, payload, retain);
			return SendMessageAsync(message, cancellationToken);
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

		private Task SendMessageAsync(MqttApplicationMessage message, CancellationToken cancellationToken)
		{
			return Client.PublishAsync(message, cancellationToken);
		}

		public void Dispose()
		{
			if (Client.IsConnected)
			{
				SendMessageAsync(GetLastWill(), CancellationToken.None).Wait(5000);
			}
		}
	}
}
