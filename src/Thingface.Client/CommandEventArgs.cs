#if (NETMF44 || NETMF43)
using Microsoft.SPOT;
#else
using System;
#endif

namespace Thingface.Client
{
    public class CommandEventArgs : EventArgs
    {
        public CommandEventArgs(string senderId, string commandName, string[] commandArgs)
        {
            SenderId = senderId;
            CommandName = commandName;
            CommandArgs = commandArgs;            
        }        

        public string SenderId { get; }

        public string CommandName { get; }

        public string[] CommandArgs { get; }
    }
}