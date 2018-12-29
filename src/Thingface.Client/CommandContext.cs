namespace Thingface.Client
{
    public class CommandContext
    {
        public CommandContext(string senderId, string commandName, string[] commandArgs)
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
