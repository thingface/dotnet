#if (NETMF44 || NETMF43)
using Microsoft.SPOT;
#else
using System;
#endif

namespace Thingface.Client
{
    public class ConnectionStateEventArgs : EventArgs
    {
        public ConnectionStateEventArgs(ConnectionState state)
        {
            NewState = state;
        }

        public ConnectionState NewState { get; }
    }

    public enum ConnectionState
    {
        Connected = 1,
        Disconnected = 0
    }
}