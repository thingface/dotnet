﻿using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Thingface.Client;

namespace Thingface.Example.Netduino
{
    public class Program
    {
        private static IThingfaceClient _thingface;
        private static Timer _timer;
        private static readonly OutputPort _led = new OutputPort(Pins.ONBOARD_LED, false);        

        public static void Main()
        {
            // Enabled DHCP
            //Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].EnableDhcp();

            // Thingface Client initialization
            _thingface = new ThingfaceClient("my-device-id", "device-secret-key");            
            _thingface.ConnectionStateChanged += ConnectionStateChanged;
            _thingface.CommandReceived += _thingface_CommandReceived;
            Debug.Print("device is connecting..");
            _thingface.Connect();
            Debug.Print("device is connected");

            Thread.Sleep(Timeout.Infinite);
        }

        private static void _thingface_CommandReceived(object sender, EventArgs e)
        {
            var commandEvent = e as CommandEventArgs;
            if (commandEvent == null)
            {
                return;
            }
            if (commandEvent.CommandName == "on")
            {
                _led.Write(true);
                Debug.Print("LED ON");
            }
            if (commandEvent.CommandName == "off")
            {
                _led.Write(false);
                Debug.Print("LED OFF");
            }
        }

        private static void ConnectionStateChanged(object sender, EventArgs e)
        {
            var connectionStateEvent = e as ConnectionStateEventArgs;
            if (connectionStateEvent!= null && connectionStateEvent.NewState == ConnectionState.Connected)
            {
                _thingface.OnCommand();
                _timer = new Timer(TimerCallback1, null, 5000, 6000);
            }
            else
            {
                _timer.Dispose();
            }
        }

        private static void TimerCallback1(object state)
        {            
            var val = 12.3;
            _thingface.SendSensorValue("temp", val);
        }
    }
}
