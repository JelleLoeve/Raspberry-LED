using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.AspNet.SignalR.Client;
using Raspberry.IO.GeneralPurpose;

namespace Raspberry_LED_Client
{
    internal class MainClass
    {
        private static GpioConnection gpio;
        private static GpioConnectionDriver driver;

        public static void Main()
        {
            Console.CancelKeyPress += delegate
            {
                Debug.Print("Exiting");
                for (var i = 0; i == 32;)
                {
                    driver.Release((ProcessorPin)i);
                    i++;
                }
                gpio.Close();
                Thread.Sleep(1000);
                Environment.Exit(0);
            };
            // Connection to the signalr hub
            Debug.Print("Connecting to http://192.168.1.100");
            var hubconnection = new HubConnection("http://192.168.1.100");
            var raspberryHub = hubconnection.CreateHubProxy("Raspberry");

            hubconnection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Print("There was an error opening the connection:{0}", task.Exception.GetBaseException());
                }
                else
                {
                    Debug.Print("Connected");
                }
            }).Wait();

            if (Helpers.IsLinux) // This can only run on the pi, windows will crash HARD
            {
                //var led1 = ConnectorPin.P1Pin05.Output().Name("led1");

                gpio = new GpioConnection();
                driver = new GpioConnectionDriver();

                //gpio.Add(led1);

                var switchButton = ConnectorPin.P1Pin03.Input().Revert().OnStatusChanged(x =>
                {
                    Debug.Print("Button Switched {0}", x ? "On" : "Off" );
                    raspberryHub.Invoke<string>("SendChangedValue", ConnectorPin.P1Pin03, x ? "On" : "Off").Wait();
                });
                gpio.Add(switchButton);
                //driver.Write(led1.Pin, false);
            }

            raspberryHub.On<string>("ChangePiLed", pinnumber => 
            { 
                Debug.Print(pinnumber);
                int ledid = int.Parse(pinnumber);
                var procpin = ((ConnectorPin) ledid).ToProcessor();
                //driver.Allocate(procpin, PinDirection.Output);
                driver.Write(procpin, !driver.Read(procpin));
                //raspberryHub.Invoke<string>("SendChangedValue", pinnumber.ToString(), driver.Read(procpin) ? "On" : "Off").Wait();
            });

            

            ServerWorkThread objThread = new ServerWorkThread();
            while (true)
            {
                objThread.HandleConnection(objThread.mySocket.Accept());
            }
        } // End of Main function
    }
}