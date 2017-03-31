using System;
using System.Threading;
using Thingface.Client;

namespace Thingface.Example
{
    public class Program
    {
        static IThingfaceClient thingface = null;

        static Timer timer = null;

        private static void TimerCallback1(object state)
        {
            Console.WriteLine("send temp");
            thingface.SendSensorValue("temp", 12.3);
        }

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

                timer = new Timer(TimerCallback1, null, 3000, 5000);
            }
            if(eventArgs.NewState == ConnectionState.Disconnected)
            {
                timer.Dispose();
                Console.WriteLine("Client is Disconnected");
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
