#if !(NETMF44 || NETMF43)
using System;
#else 
using Microsoft.SPOT;
#endif

namespace Thingface.Client
{
    /// <summary>
    /// Thingface Client
    /// </summary>
    public interface IThingfaceClient
    {
        /// <summary>
        /// Connection status
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connect to thingface server
        /// </summary>
        void Connect();

        /// <summary>
        /// disconnect from thingface server
        /// </summary>
        void Disconnect();

#if (NETMF44 || NETMF43)        
        /// <summary>
        /// Send sensor value to thingface server
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="sensorValue"></param>
        void SendSensorValue(string sensorId, object sensorValue);
#else
        /// <summary>
        /// Send sensor value to thingface server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sensorId">Sensor ID</param>
        /// <param name="sensorValue">Sensor Value</param>
        void SendSensorValue<T>(string sensorId, T sensorValue);
#endif

#if (NETMF44 || NETMF43 || MF_FRAMEWORK_VERSION_V4_3)       
        /// <summary>
        /// Subscribe to commands for current device
        /// </summary>        
        /// <param name="senderType"></param>
        /// <param name="senderId"></param>
        void OnCommand(string senderId = null);
#else
        /// <summary>
        /// Subscribe to commands for current device
        /// </summary>
        /// <param name="commandHandler"></param>
        /// <param name="senderType"></param>
        /// <param name="senderId"></param>
        void OnCommand(Func<CommandContext, int> commandHandler = null, string senderId = null);
#endif
        /// <summary>
        /// Unsubscribe from commands for current device
        /// </summary>
        void OffCommand(string senderId = null);

#if (NETMF44 || NETMF43)
        /// <summary>
        /// Connection state change event
        /// </summary>
        event EventHandler ConnectionStateChanged;

        /// <summary>
        /// Command received event
        /// </summary>
        event EventHandler CommandReceived;
#else
        /// <summary>
        /// Connection state change event
        /// </summary>
        event EventHandler<ConnectionStateEventArgs> ConnectionStateChanged;

        /// <summary>
        /// Command received event
        /// </summary>
        event EventHandler<CommandEventArgs> CommandReceived;
#endif

    }
}