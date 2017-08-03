using System;
using System.Text;

#if (NETMF44 || NETMF43)
using System.Collections;
using Json.NETMF;
using Microsoft.SPOT;
#endif

#if !(NETMF44 || NETMF43)
using System.Text.RegularExpressions;
using Newtonsoft.Json;
#endif

#if NETSTANDARD1_5
using M2Mqtt;
using M2Mqtt.Messages;
#else
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
#endif

namespace Thingface.Client
{
    public class ThingfaceClient : IThingfaceClient
    {               
        private readonly string _deviceId;
        private readonly string _secretKey;
        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSsl;
        private MqttClient _client;

#if !(NETMF44 || NETMF43)
        private const string CommandRegexString = "([ud]{1})/([a-zA-Z0-9]+)/c/([a-zA-Z0-9]+)";
        private Action<CommandContext> _commandHandler;
#endif

        public ThingfaceClient(string deviceId, string secretKey, string host = "personal.thingface.io", int port = 8883, bool enableSsl = true)
        {
#if (NETMF44 || NETMF43)
            if (StringExt.IsNullOrWhiteSpace(deviceId))
#else
            if (string.IsNullOrWhiteSpace(deviceId))
#endif
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

#if (NETMF44 || NETMF43)
            if (StringExt.IsNullOrWhiteSpace(secretKey))
#else
            if (string.IsNullOrWhiteSpace(secretKey))
#endif
            {
                throw new ArgumentNullException(nameof(secretKey));
            }

#if (NETMF44 || NETMF43)
            if(StringExt.IsNullOrWhiteSpace(host))
#else
            if (string.IsNullOrWhiteSpace(host))
#endif
            {
                throw new ArgumentNullException(nameof(host));
            }
            if (port <=0 || port > 65536)
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }
            _deviceId = deviceId;
            _secretKey = secretKey;
            _host = host;
            _port = port;
            _enableSsl = enableSsl;
            
            Initialize();
        }

#region IThingfaceClient's members

        public bool IsConnected
        {
            get { return _client != null && _client.IsConnected; }
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

            OnConnectionStateChanged(new ConnectionStateEventArgs(ConnectionState.Connected));
        }

        public void Disconnect()
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
            }
        }
#if (NETMF43 || NETMF44)
        public void SendSensorValue(string sensorId, object sensorValue)
      
#else
        public void SendSensorValue<T>(string sensorId, T sensorValue)
#endif
        {
#if (NETMF43 || NETMF44)
            if (StringExt.IsNullOrWhiteSpace(sensorId))            
#else
            if (string.IsNullOrWhiteSpace(sensorId))
#endif
            {
                throw new ArgumentNullException(nameof(sensorId));
            }

            if (sensorValue == null)
            {
                throw new ArgumentNullException(nameof(sensorValue));
            }

            if (!(sensorValue is double) &&
                !(sensorValue is int) &&
                !(sensorValue is long))
            {
                throw new ArgumentOutOfRangeException(nameof(sensorValue));
            }

            var topic = "d/" + _deviceId + "/s/" + sensorId;

#if (NETMF44 || NETMF43)
            var jsonString = "{\"v\":";
            jsonString += sensorValue;
            jsonString += "}";
            var message = Encoding.UTF8.GetBytes(jsonString);
#else
            var sensorValuePayload = new SensorPayload(sensorValue);            
            var jsonString = JsonConvert.SerializeObject(sensorValuePayload);
            var message = Encoding.UTF8.GetBytes(jsonString);
#endif

            _client.Publish(topic, message, 0, false);
        }

#if (NETMF44 || NETMF43)
        public void OnCommand(SenderType senderType = SenderType.All, string senderId = null)
        {
            if (!_client.IsConnected)
            {
                throw new Exception("Client is disconnected.");
            }
            if (senderId != null && senderType == SenderType.All)
            {
                throw new Exception("Sender type must be provided when sender ID is not null.");
            }

            var topicFilter = BuildCommandTopicFilter(senderType, senderId);
            _client.Subscribe(new[] { topicFilter }, new byte[] { 0 });
        }
#else
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
            if (!string.IsNullOrWhiteSpace(senderId) && senderId.Length > 25)
            {
                throw new ArgumentOutOfRangeException(nameof(senderId));
            }

            var topicFilter = BuildCommandTopicFilter(senderType, senderId);
            _client.Subscribe(new[] {topicFilter}, new byte[] {0});
            _commandHandler = commandHandler;
        }
#endif

        public void OffCommand(SenderType senderType = SenderType.All, string senderId = null)
        {
            if (!_client.IsConnected)
            {
                throw new Exception("Client is disconnected.");
            }
            if ((senderId != null && senderId.Trim().Length > 0) && senderType == SenderType.All)
            {
                throw new Exception("Sender type must be provided when sender ID is not null.");
            }

#if (NETMF43 || NETMF44)
            if (!StringExt.IsNullOrWhiteSpace(senderId) && senderId.Length > 25)
#else
            if (!string.IsNullOrWhiteSpace(senderId) && senderId.Length > 25)
#endif
            {
                throw new ArgumentOutOfRangeException(nameof(senderId));
            }

            var topicFilter = BuildCommandTopicFilter(senderType, senderId);
            _client.Unsubscribe(new[] { topicFilter });

#if !(NETMF44 || NETMF43)
            _commandHandler = null;
#endif
        }

#if (NETMF44 || NETMF43)
        public event EventHandler ConnectionStateChanged;
        public event EventHandler CommandReceived;
#else
        public event EventHandler<ConnectionStateEventArgs> ConnectionStateChanged;
        public event EventHandler<CommandEventArgs> CommandReceived;
#endif


#endregion

#region Private

        private void Initialize()
        {
#if (NETMF44 || NETMF43)
            if (_enableSsl)
            {
                _client = new MqttClient(_host, _port, true, null, null, MqttSslProtocols.TLSv1_1);
            }
            else
            {
                _client = new MqttClient(_host, _port, false, null, null, MqttSslProtocols.None);
            }
#else
            if (_enableSsl)
            {
                _client = new MqttClient(_host, _port, true, MqttSslProtocols.TLSv1_2, null, null);
            }
            else
            {
                _client = new MqttClient(_host, _port, false, MqttSslProtocols.None, null, null);
            }
#endif
        }

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

#if (NETMF44 || NETMF43)
            if (senderId != null && senderId.Trim().Length > 0)
#else
            if (!string.IsNullOrWhiteSpace(senderId))
#endif
            {
                return senderTypeStr + "/" + senderId + "/c/" + _deviceId;
            }

            return senderTypeStr + "/+/c/" + _deviceId;
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

        private void _client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
#if (NETMF44 || NETMF43)
            var topicParts = e.Topic.Split('/');           
            if (topicParts.Length == 4 && topicParts[1] == "c")
            {
                string payloadString = new string(Encoding.UTF8.GetChars(e.Message));
                Hashtable command = JsonSerializer.DeserializeString(payloadString) as Hashtable;

                var senderType = ParseSenderType(topicParts[0]);
                var senderId = topicParts[2];
                var commandName = command["c"] as string;
                var commandArgs = (command["a"] as ArrayList).ToArray();

                OnCommandReceived(new CommandEventArgs(senderType, senderId, commandName, (string[]) commandArgs));
            }
#else
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
#endif
        }

        private void _client_ConnectionClosed(object sender, EventArgs eventArgs)
        {
            OnConnectionStateChanged(new ConnectionStateEventArgs(ConnectionState.Disconnected));
        }

#endregion

#if (NETMF44 || NETMF43)
        protected virtual void OnConnectionStateChanged(EventArgs e)
#else
        protected virtual void OnConnectionStateChanged(ConnectionStateEventArgs e)
#endif

        {
            if (ConnectionStateChanged != null)
            {
                ConnectionStateChanged.Invoke(this, e);
            }
        }

#if (NETMF44 || NETMF43)
        protected virtual void OnCommandReceived(EventArgs e)
#else
        protected virtual void OnCommandReceived(CommandEventArgs e)
#endif
        {
            if (CommandReceived != null)
            {
                CommandReceived.Invoke(this, e);
            }
        }
    }
}
