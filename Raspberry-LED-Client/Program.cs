using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Raspberry.IO.GeneralPurpose;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static Raspberry_LED_Client.Helpers;

namespace Raspberry_LED_Client
{
    class MainClass
    {
        static byte[] Buffer { get; set; }
        static Socket socket;
        static Socket accepted = null;
        
        public static int ServerPort = 12345;
        


        public static void Main()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(0, ServerPort));
            while (true)
            {
                socket.Listen(100);
                Console.WriteLine("Awaiting Data");
                accepted = socket.Accept();
                Buffer = new byte[accepted.SendBufferSize];
                int bytesRead = accepted.Receive(Buffer);
                byte[] formatted = new byte[bytesRead];
                for (int i = 0; i < bytesRead; i++)
                {
                    formatted[i] = Buffer[i];
                }

                string strData = Encoding.ASCII.GetString(formatted);
                string[] typcom = strData.Split(':');
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
                            Console.WriteLine("Let it play, let is play, let it play");
                            Console.WriteLine(command);
                            Process proc = new Process();
                            proc.EnableRaisingEvents = false;
                            proc.StartInfo.FileName = @"mplayer";
                            proc.StartInfo.Arguments = command;
                            proc.Start();
                            break;
                        case "SYS":
                            Console.WriteLine("System command");
                            break;
                        default:
                            Console.WriteLine("Commandtype '{0}' is not configured", commandtype);
                            break;
                    }
                    Console.WriteLine(strData + Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Can't parse \"" + strData + "\" as a command.");
                }
                Thread.Sleep(50);
            }// End of while loop
        } // End of Main function

        private static void changeLed(LEDS LED, bool state)
        {
            var led = ((ConnectorPin) LED).Output();
            var connection = new GpioConnection(led);
            connection.Toggle(led);
        }

        private static bool PlayMusic(string musicFile)
        {
			Process proc = new Process();
			proc.EnableRaisingEvents=false; 
			proc.StartInfo.FileName = "mplayer";
			proc.StartInfo.Arguments = "-t wav /home/pi/rc/media/honk.wav";
			proc.Start();
            return false;
        }
    }
}
