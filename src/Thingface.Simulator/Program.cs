using System;
using Thingface.Client;

namespace Thingface.Simulator
{
    public class Program
    {

        private static void connectionChanged(object sender, ConnectionStateEventArgs eventArgs)
        {
            Console.WriteLine("Connection {0}", eventArgs.NewState);
        }

        private static void CommandReceived(object sender, CommandEventArgs eventArgs)
        {
            Console.WriteLine("event {0} sent command {1}", eventArgs.Sender, eventArgs.CommandName);
        }

        public static void Main(string[] args)
        {
            var thingface = new MqttThingfaceClient("mydevice", "qEuzwhBLEPHEP25fts01wAnLHpe7GS", "dev-app.thingface.io");
            thingface.ConnectionStateChanged += connectionChanged;
            thingface.CommandReceived += CommandReceived;
            thingface.Connect();

            thingface.OnCommand((cmd)=>{
                Console.WriteLine("{0} sent command {1}", cmd.Sender, cmd.CommandName);
            });
            thingface.SendSensorValue("s1", 123);
            Console.WriteLine("Simulator started.");

            Console.Read();

            thingface.OffCommand();

            thingface.Disconnect();
        }
    }
}
