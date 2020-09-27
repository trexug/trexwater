namespace TrexWater.Messaging
{
	public class MqttOptions
	{
		public const string MQTT = "Mqtt";
		public string ClientName { get; set; } = "TrexWater";
		public string Host { get; set; }
		public int? Port { get; set; }
		public bool UseTls { get; set; } = true;
		public string Username { get; set; }
		public string Password { get; set; }
		public int ReconnectTimeoutMiliseconds { get; set; } = 5000;
		public string LocalDeviceName { get; set; } = "wdev";
		public bool HasCredentials => !string.IsNullOrEmpty(Username);
	}
}
