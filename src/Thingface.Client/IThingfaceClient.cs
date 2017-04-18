#if !(NETMF44 || NETMF43)
using System;
#else 
using Microsoft.SPOT;
#endif

namespace Thingface.Client
{
    public interface IThingfaceClient
    {
        bool IsConnected { get; }

        void Connect();

        void Disconnect();

        void SendSensorValue(string sensorId, double sensorValue);

#if (NETMF44 || NETMF43 || MF_FRAMEWORK_VERSION_V4_3)
        void OnCommand(SenderType senderType = SenderType.All, string senderId = null);
#else
        void OnCommand(Action<CommandContext> commandHandler = null, SenderType senderType = SenderType.All, string senderId = null);
#endif

        void OffCommand(SenderType senderType = SenderType.All, string senderId = null);

#if (NETMF44 || NETMF43)
        event EventHandler ConnectionStateChanged;

        event EventHandler CommandReceived;
#else
        event EventHandler<ConnectionStateEventArgs> ConnectionStateChanged;

        event EventHandler<CommandEventArgs> CommandReceived;
#endif

    }
}