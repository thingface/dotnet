using Microsoft.SPOT;
using Thingface.Client;

namespace Thingface.Example.Netduino
{
    public class Program
    {
        private static ThingfaceClient thingface;

        public static void Main()
        {
            thingface = new ThingfaceClient("753f51106f49401a", "YcKtLbRHtB1excLvZsXqZ2RdODiL5J");
            //thingface.ConnectionStateChanged += ConnectionStateChanged;
            Debug.Print("client is connecting..");
            thingface.Connect();
            Debug.Print("client is connected");
        }      
    }
}
