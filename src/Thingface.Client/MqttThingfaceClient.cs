using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using M2Mqtt;
using M2Mqtt.Messages;
using Newtonsoft.Json;

namespace Thingface.Client
{
    public class MqttThingfaceClient : IThingfaceClient
    {
        private readonly string _deviceId;
        private readonly string _secretKey;
        private readonly string _host;
        private readonly int _port;

        private readonly MqttClient _client;
        private Action<CommandContext> _commandHandler;

        public MqttThingfaceClient(string deviceId, string secretKey, string host = "personal.thingface.io", int port = 8883)
        {
            _deviceId = deviceId;
            _secretKey = secretKey;
            _host = host;
            _port = port;
            _client = new MqttClient(_host, _port, true, MqttSslProtocols.TLSv1_2, null, null);
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

            OnConnectionState(ConnectionState.Connected);
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
            var topic = "tf/d/" + _deviceId + "/" + sensorId;
            var jsonString = JsonConvert.SerializeObject(sensorValuePayload);
            var message = Encoding.UTF8.GetBytes(jsonString);
            _client.Publish(topic, message, 0, false);
        }

        public void OnCommand(Action<CommandContext> commandHandler = null, string sender = null)
        {
            if (!_client.IsConnected)
            {
                throw new Exception("Client is disconnected.");
            }
            string topic = "tf/c/+/"+_deviceId;
            if (!string.IsNullOrWhiteSpace(sender))
            {
                topic = "tf/c/"+sender+"/"+_deviceId;
            }
            _client.Subscribe(new [] {topic}, new byte[] {0});
            _commandHandler = commandHandler;
        }

        public void OffCommand(string sender = null)
        {
            if (!_client.IsConnected)
            {
                throw new Exception("Client is disconnected.");
            }
            string topicFilter = "tf/c/+/"+_deviceId;
            if (!string.IsNullOrWhiteSpace(sender))
            {
                topicFilter = "tf/c/"+sender+"/"+_deviceId;
            }
            _client.Unsubscribe(new [] {topicFilter});
            _commandHandler = null;
        }

        public event EventHandler<ConnectionStateEventArgs> ConnectionStateChanged;

        public event EventHandler<CommandEventArgs> CommandReceived;

        #endregion

        #region Private

        private void OnConnectionState(ConnectionState state){
            if (ConnectionStateChanged!=null)
            {
                ConnectionStateChanged(this, new ConnectionStateEventArgs(state));
            }
        }

        private void OnCommandReceived(string sender, string commandName, string[] commandArgs)
        {
            if (CommandReceived!=null)
            {
                CommandReceived(this, new CommandEventArgs(sender, commandName, commandArgs));
            }
        }

        private void _client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic.StartsWith("tf/c/"))
            {
                string payloadString = Encoding.UTF8.GetString(e.Message);
                var command = JsonConvert.DeserializeObject<CommandPayload>(payloadString);
                var regex = new Regex("tf/c/([a-zA-Z0-9]+)/([a-zA-Z0-9]+)");
                var matches = regex.Match(e.Topic);
                var commandSender = matches.Groups[1].Value;
                if (_commandHandler!=null)
                {
                    _commandHandler(new CommandContext(commandSender, command.Name, command.Args));
                }
                OnCommandReceived(commandSender, command.Name, command.Args);
            }
        }

        private void _client_ConnectionClosed(object sender, EventArgs eventArgs)
        {
            OnConnectionState(ConnectionState.Disconnected);
        }

        #endregion
    }
}
