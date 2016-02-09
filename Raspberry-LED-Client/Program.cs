using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR.Client;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;

namespace Raspberry_LED_Client
{
    internal class MainClass
    {
        private static GpioConnection gpio;
        private static GpioConnectionDriver driver;
        private static I2cDriver I2CDriver;
        private static I2cDeviceConnection ArduinoConnection;
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
                Console.WriteLine("Stopping the program");
                HubConnection.Closed -= StartHubConnection;
                HubConnection.Closed += null;
                HubConnection.Stop();
                Console.WriteLine("Stopped SignalR communication");
                for (var i = 0; i == 32;)
                {
                    string str = $"gpio{i}";
                    if (Directory.Exists(Path.Combine("/sys/class/gpio", str)))
                    {
                        driver.Release((ProcessorPin) i);
                    }
                    i++;
                }
                gpio.Close();
                Console.WriteLine("Stopped driver allocating");
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
            

            gpio = new GpioConnection();
            driver = new GpioConnectionDriver();
            I2CDriver = new I2cDriver(ConnectorPin.P1Pin3.ToProcessor(), ConnectorPin.P1Pin5.ToProcessor());
            ArduinoConnection = I2CDriver.Connect(0x04);
            var switchButton = ConnectorPin.P1Pin13.Input().Revert().OnStatusChanged(x =>
            {
                Console.WriteLine(x);
                RaspberryHub.Invoke("SendChangedValue", ConnectorPin.P1Pin37, x ? "On" : "Off");
            });
            var doorSensor = ConnectorPin.P1Pin7.Input().PullUp().OnStatusChanged(x =>
            {
                RaspberryHub.Invoke("SendChangedValue", ConnectorPin.P1Pin7, x ? "Open" : "Closed");
            });
            var motionSensor = ConnectorPin.P1Pin13.Input().OnStatusChanged(x =>
            {
                RaspberryHub.Invoke("SendChangedValue", ConnectorPin.P1Pin11, x ? "Detected" : "Not detected");
                Console.WriteLine( DateTime.Now + ":Motion {0}", x ? "Detected" : "Not detected");
            });
//            gpio.Add(switchButton);
//            gpio.Add(doorSensor);
//            gpio.Add(motionSensor);

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

            RaspberryHub.On<string, string>("SetupConfig", (pinnumber, type) =>
            {
                int pin = int.Parse(pinnumber);
                if (pin > 7)
                {
                    Action<bool> onstatusaction =
                        b =>
                        {
                            RaspberryHub.Invoke("SendChangedValue", pin,
                                driver.Read(((ConnectorPin) pin).ToProcessor()) ? "On" : "Off");
                        };
                    string str = string.Format("gpio{0}", ((ConnectorPin)pin).ToProcessor().ToString().Replace("Pin0", "").Replace("Pin", ""));
                    Console.WriteLine(str);
                    if (!Directory.Exists(Path.Combine("/sys/class/gpio", str)))
                    {
                        Console.WriteLine("Adding button");
                        var button = CreatePinConfig.CreateOutputPinConfiguration((ConnectorPin) pin, onstatusaction, "Button");
                        gpio.Add(button);
                    }
                }
            });

            RaspberryHub.On<int, string>("GetPinStatus", (pin, type) =>
            {
                driver.Read(((ConnectorPin) pin).ToProcessor());
                string status = string.Empty;
                if (type.Equals("Button"))
                {
                    status = driver.Read(((ConnectorPin) pin).ToProcessor()) ? "Pressed" : "Not pressed";
                }
                if (type.Equals("LED"))
                {
                    status = driver.Read(((ConnectorPin) pin).ToProcessor()) ? "On" : "Off";
                }
                if (type.Equals("Door sensor"))
                {
                    status = driver.Read(((ConnectorPin)pin).ToProcessor()) ? "Open" : "Closed";
                }

                RaspberryHub.Invoke("SendChangedValue", pin, status);
            });
            

            ServerWorkThread objThread = new ServerWorkThread();
            SendToArduino(1);
            while (true)
            {
                Thread.Sleep(50);
                ReadFromArduino();
                //objThread.HandleConnection(objThread.mySocket.Accept());
            }
        } // End of Main function

        private static void SendToArduino(object dataToSend)
        {
            var data = System.Text.Encoding.UTF8.GetBytes(dataToSend+"");
            ArduinoConnection.Write(data);
        }

        private static string ReadFromArduino()
        {
            byte[] recievedBytes = ArduinoConnection.Read(52);
            string recievedData = null;
            int len = 52;
            while (len > 0 && recievedBytes[len - 1] == 0)
            {
                len--;
            }
            byte[] cropped = new byte[len];
            if (len > 0)
            {
                Array.Copy(recievedBytes, 0, cropped, 0, len);
            }
            recievedBytes = cropped;
            recievedData = System.Text.Encoding.UTF8.GetString(recievedBytes);
            recievedData = recievedData.Replace("�", "");
            Console.WriteLine(recievedData);
            return recievedData;
        }
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