using System;
using System.Threading;
using System.Threading.Tasks;
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
            thingface.SendSensorValue("s001", val);
            Console.WriteLine($"sent s001 = {val}");
        }        

        private static void ConnectionStateChanged(object sender, ConnectionStateEventArgs eventArgs)
        {
            if (eventArgs.NewState == ConnectionState.Connected)
            {
                Console.WriteLine("device is connected");
                var thingface = (IThingfaceClient)sender;
                //thingface.OnCommand(CommandHandler, SenderType.User, "mirrobozik");
                //thingface.OnCommand(CommandHandler, SenderType.User);
                thingface.OnCommand((context) =>
                {
                    Console.WriteLine($"Received command '{context.CommandName}' from '{context.SenderId}'");
                    Task.Delay(3000).GetAwaiter().GetResult();
                    return 0;
                });

                //timer = new Timer(TimerCallback1, null, 6000, 7000);
            }
            if(eventArgs.NewState == ConnectionState.Disconnected)
            {
                timer?.Dispose();
                Console.WriteLine("device is disconnected");
            }
        }

        public static void Main(string[] args)
        {            
            thingface = new ThingfaceClient("0bdaf71167754aa4", "UBH4T2MbgkOZ0wxEZEC0jjkkPZfFsJ", "127.0.0.1", 1883, false);
            thingface.ConnectionStateChanged += ConnectionStateChanged;
            Console.WriteLine("device is connecting..");
            thingface.Connect();

            Console.Read();

            Console.WriteLine("off command..");
            thingface.OffCommand();                    

            Task.Delay(2000).GetAwaiter().GetResult();
            Console.WriteLine("disconnecting..");
            thingface.Disconnect();            
        }
    }
}
