using System;

namespace Thingface.Client
{
    public interface IThingfaceClient
    {
        bool IsConnected { get; }    

        void Connect();        

        void Disconnect();

        void SendSensorValue(string sensorId, double sensorValue);

        void OnCommand(Action<CommandContext> commandHandler = null, SenderType senderType = SenderType.All, string senderId = null);

        void OffCommand(SenderType senderType = SenderType.All, string senderId = null);

        event EventHandler<ConnectionStateEventArgs> ConnectionStateChanged;

        event EventHandler<CommandEventArgs> CommandReceived;
    }
}