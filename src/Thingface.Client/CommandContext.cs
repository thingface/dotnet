namespace Thingface.Client
{
    public class CommandContext
    {
        public CommandContext(string sender, string commandName, string[] commandArgs)
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
