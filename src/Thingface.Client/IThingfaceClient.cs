using System;
using System.Threading.Tasks;

namespace Thingface.Client
{
    public interface IThingfaceClient
    {
        bool IsConnected { get; }

        Task ConnectAsync();

        void Connect();

        Task DisconnectAsync();

        void Disconnect();

        Task SendSensorValueAsync(string sensorId, double sensorValue);

        void SendSensorValue(string sensorId, double sensorValue);

        void OnCommand(Action<CommandContext> commandHandler = null, SenderType senderType = SenderType.All, string senderId = null);

        void OffCommand(SenderType senderType = SenderType.All, string senderId = null);

        event EventHandler<ConnectionStateEventArgs> ConnectionStateChanged;

        event EventHandler<CommandEventArgs> CommandReceived;
    }
}