namespace Thingface.Client
{
    public class CommandContext
    {
        public CommandContext(SenderType senderType, string senderId, string commandName, string[] commandArgs)
        {
            SenderId = senderId;
            CommandName = commandName;
            CommandArgs = commandArgs;
            SenderType = senderType;
        }

        public SenderType SenderType { get; }

        public string SenderId { get; }

        public string CommandName { get; }

        public string[] CommandArgs { get; }
    }
}
