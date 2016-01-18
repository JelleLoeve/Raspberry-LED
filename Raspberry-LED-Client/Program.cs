using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.AspNet.SignalR.Client;
using Raspberry.IO.GeneralPurpose;
using static Raspberry_LED_Client.Helpers;

namespace Raspberry_LED_Client
{
    internal class MainClass
    {
        private static byte[] Buffer { get; set; }

        public static int ServerPort = 20020;

        public static string ftpPath = !IsLinux ? "C:/inetpub/ftproot/" : "/home/pi/music/Raspberry-LED/";
        private static GpioConnection gpio;
        private static GpioConnectionDriver driver;
        public static void Main()
        {
            Console.CancelKeyPress += delegate
            {
                Console.WriteLine("Exiting");
                gpio.Close();
                Thread.Sleep(1000);
                Environment.Exit(0);
            };
            if (IsLinux)
            {
                var led1 = ConnectorPin.P1Pin05.Output().Name("led1");

                gpio = new GpioConnection();
                driver = new GpioConnectionDriver();
                gpio.Add(led1);

                var switchButton = ConnectorPin.P1Pin03.Input().Revert().OnStatusChanged(x =>
                {
                    Console.WriteLine($"Button Switched {x}", x ? "HIGH" : "LOW" );
                    gpio.Pins["led1"].Toggle();

                });
                gpio.Add(switchButton);
                driver.Write(led1.Pin, false);
            }
            
            


            Console.WriteLine("Connecting to http://192.168.1.100");
            var hubconnection = new HubConnection("http://192.168.1.100");
            var raspberryHub = hubconnection.CreateHubProxy("Raspberry");
            
            hubconnection.Start().ContinueWith(task =>
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
            
            
            
            
            raspberryHub.On<string>("ChangePiLed", lednumber => {
                Console.WriteLine(lednumber);
                int ledid = int.Parse(lednumber);
                gpio.Pins[((ConnectorPin)ledid).ToProcessor()].Toggle();
                
            });
            
            //raspberryHub.On<string, string>("ChangePiLed", (PinNumber, IsOn) => Console.WriteLine($"{PinNumber}   {IsOn}"));

            //ServerWorkThread objThread = new ServerWorkThread();
            while (true)
            {
                //objThread.HandleConnection(objThread.mySocket.Accept());

            }


        } // End of Main function

        public class ServerWorkThread
        {
            public Socket mySocket;

            public ServerWorkThread()
            {
                IPEndPoint objEnpoint = new IPEndPoint(0, ServerPort);
                mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mySocket.Bind(objEnpoint);
                mySocket.Listen(100);
                Console.WriteLine("Awaiting Data");
            }

            public void HandleConnection(Socket iIncomingSocket)
            {
                Thread worker = new Thread(RecieveAndSend);
                worker.Start(iIncomingSocket);
                worker.Join();
            }

            public void RecieveAndSend(object iIncoming)
            {
                Socket objSocket = (Socket)iIncoming;
                byte[] bytes = new byte[1024];
                string strSend = string.Empty;
                Buffer = new byte[objSocket.SendBufferSize];
                int bytesRead = objSocket.Receive(Buffer);
                byte[] formatted = new byte[bytesRead];
                for (int i = 0; i < bytesRead; i++)
                {
                    formatted[i] = Buffer[i];
                }

                string strReceived = Encoding.UTF8.GetString(formatted);

                string[] typcom = strReceived.Split(':');
                if (typcom.Length == 2 || typcom.Length == 3)
                {
                    string commandtype = typcom[0];
                    string command = typcom[1];
                    string parameters = null;
                    if (typcom.Length == 3)
                    {
                        parameters = typcom[2];
                    }
                    switch (commandtype)
                    {
                        case "PLAY":
                            if (File.Exists(ftpPath + command))
                            {
                                Console.WriteLine("Let it play, let is play, let it play");
                                Console.WriteLine(command);
                                Process proc = new Process();
                                proc.EnableRaisingEvents = false;
                                if (!IsLinux)
                                {
                                    proc.StartInfo.FileName = @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe";
                                    proc.StartInfo.Arguments = "\"file:///" + ftpPath + command + "\"";
                                }
                                else
                                {
                                    proc.StartInfo.FileName = "mplayer";
                                    proc.StartInfo.Arguments = "\"" + ftpPath + command + "\"";
                                }
                                proc.Start();
                            }
                            else
                            {
                                strSend = "NoFile";
                            }
                            break;
                        default:
                            strSend = $"Commandtype '{commandtype}' is not configured";
                            break;
                    } // End of switch

                    objSocket.Send(Encoding.UTF8.GetBytes(strSend));
                    Console.WriteLine(strReceived + Environment.NewLine);
                    objSocket.Close();
                }
            }
        }
    }
}