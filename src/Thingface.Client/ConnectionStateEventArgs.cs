namespace Thingface.Client
{
    public class ConnectionStateEventArgs
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