using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using M2Mqtt;
using M2Mqtt.Messages;
using Newtonsoft.Json;

namespace Thingface.Client
{
    public class ThingfaceClient : IThingfaceClient
    {
        private const string CommandRegexString = "([ud]{1})/c/([a-zA-Z0-9]+)/([a-zA-Z0-9]+)";

        private readonly string _deviceId;
        private readonly string _secretKey;
        private readonly string _host;
        private readonly int _port;

        private readonly MqttClient _client;
        private Action<CommandContext> _commandHandler;

        public ThingfaceClient(string deviceId, string secretKey, string host = "personal.thingface.io", int port = 8883, bool enableSsl = true)
        {
            _deviceId = deviceId;
            _secretKey = secretKey;
            _host = host;
            _port = port;
            if (enableSsl)
            {
                _client = new MqttClient(_host, _port, true, MqttSslProtocols.TLSv1_2, null, null);
            }
            else
            {
                _client = new MqttClient(_host, _port, false, MqttSslProtocols.None, null, null);
            }
        }

        #region IThingfaceClient's members

        public bool IsConnected => _client != null && _client.IsConnected;

        public Task ConnectAsync()
        {
            return Task.Factory.StartNew(Connect);
        }

        public void Connect()
        {
            var response = _client.Connect(_deviceId, _deviceId, _secretKey, true, 60);
            if (response > 0)
            {
                throw new Exception("Cannot connect to thingface gateway");
            }

            _client.ConnectionClosed += _client_ConnectionClosed;
            _client.MqttMsgPublishReceived += _client_MqttMsgPublishReceived;

            OnConnectionState(Thingface.Client.ConnectionState.Connected);
        }

        public Task DisconnectAsync()
        {
            return Task.Factory.StartNew(Disconnect);
        }

        public void Disconnect()
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
            }
        }

        public Task SendSensorValueAsync(string sensorId, double sensorValue)
        {
            return Task.Factory.StartNew(() => SendSensorValue(sensorId, sensorValue));
        }

        public void SendSensorValue(string sensorId, double sensorValue)
        {
            var sensorValuePayload = new SensorValue(sensorValue);
            var topic = "d/d/" + _deviceId + "/" + sensorId;
            var jsonString = JsonConvert.SerializeObject(sensorValuePayload);
            var message = Encoding.UTF8.GetBytes(jsonString);
            _client.Publish(topic, message, 0, false);
        }

        public void OnCommand(Action<CommandContext> commandHandler = null, SenderType senderType = SenderType.All, string senderId = null)
        {
            if (!_client.IsConnected)
            {
                throw new Exception("Client is disconnected.");
            }
            if (!string.IsNullOrWhiteSpace(senderId) && senderType==SenderType.All)
            {
                throw new Exception("Sender type must be provided when sender ID is not null.");
            }

            var topicFilter = BuildCommandTopicFilter(senderType, senderId);
            _client.Subscribe(new[] {topicFilter}, new byte[] {0});
            _commandHandler = commandHandler;
        }

        public void OffCommand(SenderType senderType = SenderType.All, string senderId = null)
        {
            if (!_client.IsConnected)
            {
                throw new Exception("Client is disconnected.");
            }
            if (!string.IsNullOrWhiteSpace(senderId) && senderType == SenderType.All)
            {
                throw new Exception("Sender type must be provided when sender ID is not null.");
            }

            var topicFilter = BuildCommandTopicFilter(senderType, senderId);
            _client.Unsubscribe(new[] { topicFilter });
            _commandHandler = null;
        }

        public event EventHandler<ConnectionStateEventArgs> ConnectionStateChanged;

        public event EventHandler<CommandEventArgs> CommandReceived;

        #endregion

        #region Private

        private string BuildCommandTopicFilter(SenderType senderType, string senderId)
        {
            string senderTypeStr = "+";
            switch (senderType)
            {
                case SenderType.User:
                    senderTypeStr = "u";
                    break;
                case SenderType.Device:
                    senderTypeStr = "d";
                    break;
            }

            if (!string.IsNullOrWhiteSpace(senderId))
            {
                return senderTypeStr + "/c/" + senderId + "/" + _deviceId;
            }

            return senderTypeStr + "/c/+/" + _deviceId;
        }

        private SenderType ParseSenderType(string senderType)
        {
            if (senderType == "d")
            {
                return SenderType.Device;
            }
            if (senderType == "u")
            {
                return SenderType.User;
            }
            throw new ArgumentOutOfRangeException(nameof(senderType));
        }

        private void OnConnectionState(ConnectionState state)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateEventArgs(state));
        }

        private void _client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var commandRegex = new Regex(CommandRegexString);
            if (commandRegex.IsMatch(e.Topic))
            {
                string payloadString = Encoding.UTF8.GetString(e.Message);
                var command = JsonConvert.DeserializeObject<CommandPayload>(payloadString);

                var matches = commandRegex.Match(e.Topic);
                var senderType = ParseSenderType(matches.Groups[1].Value);
                var senderId = matches.Groups[2].Value;

                _commandHandler?.Invoke(new CommandContext(senderType, senderId, command.Name, command.Args));
                CommandReceived?.Invoke(this, new CommandEventArgs(senderType, senderId, command.Name, command.Args));
            }
        }

        private void _client_ConnectionClosed(object sender, EventArgs eventArgs)
        {
            OnConnectionState(ConnectionState.Disconnected);
        }

        #endregion
    }
}
