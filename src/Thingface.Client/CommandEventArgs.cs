using System;

namespace Thingface.Client
{
    public class CommandEventArgs : EventArgs
    {
        public CommandEventArgs(string sender, string commandName, string[] commandArgs)
        {
            Sender = sender;
            CommandName = commandName;
            CommandArgs = commandArgs;
        }

        public string Sender { get; }

        public string CommandName { get; }

        public string[] CommandArgs { get; }
    }
}