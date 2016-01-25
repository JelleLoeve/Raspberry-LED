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
        public static HubConnection HubConnection;
        public static IHubProxy RaspberryHub;
        public static void Main()
        {
            Console.Title = "Raspberry-LED Domotica client";
            if (!Helpers.IsLinux)
            {
                Console.WriteLine("Sorry, almost everything in this script can only run on the Raspberry Pi.");
                Console.WriteLine("Press enter to close the script.");
                Console.Read();
                Environment.Exit(0);
            }
            Console.CancelKeyPress += delegate
            {
                Console.WriteLine("Stopping the programm");
                HubConnection.Closed += null;
                HubConnection.Stop();
                driver.Release(ConnectorPin.P1Pin03.ToProcessor());
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
            HubConnection = new HubConnection("http://192.168.1.100");
            RaspberryHub = HubConnection.CreateHubProxy("Raspberry");
            // If the server decides to close the connection we need to start it again
            HubConnection.Closed += StartHubConnection;
            // Starts the connection
            StartHubConnection();

            if (Helpers.IsLinux) // This can only run on the pi, windows will crash HARD
            {

                gpio = new GpioConnection();
                driver = new GpioConnectionDriver();

                var switchButton = ConnectorPin.P1Pin03.Input().Revert().OnStatusChanged(x =>
                {
                    RaspberryHub.Invoke("SendChangedValue", ConnectorPin.P1Pin03, x ? "On" : "Off");
                });
                var doorSensor = ConnectorPin.P1Pin7.Input().PullUp().OnStatusChanged(x =>
                {
                    RaspberryHub.Invoke("SendChangedValue", ConnectorPin.P1Pin7, x ? "Open" : "Closed");
                });
                var motionSensor = ConnectorPin.P1Pin11.Input().OnStatusChanged(x =>
                {
                    RaspberryHub.Invoke("SendChangedValue", ConnectorPin.P1Pin11, x ? "Detected" : "Not detected");
                    Console.WriteLine( DateTime.Now + ":Motion {0}", x ? "Detected" : "Not detected");
                });
                gpio.Add(switchButton);
                gpio.Add(doorSensor);
                gpio.Add(motionSensor);
            }

            RaspberryHub.On<string>("ChangePiLed", pinnumber => 
            { 
                int ledid = int.Parse(pinnumber);
                var procpin = ((ConnectorPin) ledid).ToProcessor();
                
                string str = string.Format("gpio{0}",procpin.ToString().Replace("Pin0","").Replace("Pin",""));
                if (!Directory.Exists(Path.Combine("/sys/class/gpio", str)))
                {
                    Console.WriteLine($"OutputPin {procpin} is not allocated!\nAllocating now.");
                    driver.Allocate(procpin, PinDirection.Output);
                    Console.WriteLine("Pin allocated");
                }
                driver.Write(procpin, !driver.Read(procpin));
                RaspberryHub.Invoke("SendChangedValue", pinnumber, driver.Read(procpin) ? "On" : "Off");
            });
            

            ServerWorkThread objThread = new ServerWorkThread();
            while (true)
            {
                objThread.HandleConnection(objThread.mySocket.Accept());
            }
        } // End of Main function

        private static void StartHubConnection()
        {
            HubConnection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error opening the connection:{0}", task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine("Connected");
                }
            }).Wait();
        }
    }
}