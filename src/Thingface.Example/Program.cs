using System;
using System.Threading;
using Thingface.Client;

namespace Thingface.Example
{
    public class Program
    {
        static IThingfaceClient thingface = null;

        static Timer timer = null;
        static Random random = new Random(123);

        private static void TimerCallback1(object state)
        {                        
            var val = random.NextDouble()*10;
            thingface.SendSensorValue("temp", val);
            Console.WriteLine($"sent temp = {val}");
        }

        private static void CommandHandler(CommandContext context){
            Console.WriteLine($"Received Command: '{context.CommandName}' with Args: '{string.Join(",", context.CommandArgs)}'");
            if (context.CommandName == "say")
            {
                Console.WriteLine(context.CommandArgs[0]);
            }
        }

        private static void ConnectionStateChanged(object sender, ConnectionStateEventArgs eventArgs)
        {
            if (eventArgs.NewState == ConnectionState.Connected)
            {
                Console.WriteLine("client is connected");
                var thingface = (IThingfaceClient)sender;
                //thingface.OnCommand(CommandHandler, SenderType.User, "mirrobozik");
                //thingface.OnCommand(CommandHandler, SenderType.User);
                thingface.OnCommand(CommandHandler);

                //timer = new Timer(TimerCallback1, null, 6000, 7000);
            }
            if(eventArgs.NewState == ConnectionState.Disconnected)
            {
                //timer.Dispose();
                Console.WriteLine("client is disconnected");
            }
        }

        public static void Main(string[] args)
        {
            //thingface = new ThingfaceClient("my-device-id", "my-device-secret-key");
            thingface = new ThingfaceClient("d71b43f9bce747cf", "799mqfgV156ZscgZg3QVE1fYOTjwPN", "dev-app.thingface.io");
            thingface.ConnectionStateChanged += ConnectionStateChanged;
            Console.WriteLine("client is connecting..");
            thingface.Connect();

            Console.Read();

            thingface.Disconnect();            
        }
    }
}
