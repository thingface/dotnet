using System;
using Thingface.Client;

namespace Thingface.Example
{
    public class Program
    {
        private static void CommandHandler(CommandContext context){
            if (context.CommandName == "say")
            {
                Console.WriteLine(context.CommandArgs[0]);
            }
        }

        private static void ConnectionStateChanged(object sender, ConnectionStateEventArgs eventArgs)
        {
            if (eventArgs.NewState == ConnectionState.Connected)
            {
                var thingface = (IThingfaceClient)sender;
                thingface.OnCommand(CommandHandler);
                thingface.SendSensorValue("temp", 12.3);
            }
        }

        public static void Main(string[] args)
        {
            var thingface = new ThingfaceClient("mydevice", "secret-here", "my-app.thingface.io");
            thingface.ConnectionStateChanged += ConnectionStateChanged;
            thingface.Connect();

            Console.Read();

            thingface.Disconnect();
        }
    }
}
