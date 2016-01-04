using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Raspberry.IO.GeneralPurpose;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static Raspberry_LED_Client.Helpers;

namespace Raspberry_LED_Client
{
    internal class MainClass
    {
        private static byte[] Buffer { get; set; }
        private static Socket socket;
        private static Socket accepted = null;

        public static int ServerPort = 20020;

        public static string ftpPath = !IsLinux ? "C:/inetpub/ftproot/" : "/home/pi/music/Raspberry-LED/";

        public static void Main()
        {

            ServerWorkThread objThread = new ServerWorkThread();
            Console.WriteLine("Awaiting Data");
            while (true)
            {
                objThread.HandleConnection(objThread.mySocket.Accept());
            }
//            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//            socket.Bind(new IPEndPoint(0, ServerPort));
//            while (true)
//            {
//                socket.Listen(100);
//                Console.WriteLine("Awaiting Data");
//                accepted = socket.Accept();
//                Buffer = new byte[accepted.SendBufferSize];
//                int bytesRead = accepted.Receive(Buffer);
//                byte[] formatted = new byte[bytesRead];
//                for (int i = 0; i < bytesRead; i++)
//                {
//                    formatted[i] = Buffer[i];
//                }
//
//                strData = Encoding.ASCII.GetString(formatted);
//                
//                }
//                else
//                {
//                    Console.WriteLine("Can't parse \"" + strData + "\" as a command.");
//                }
//                Console.WriteLine(strData + Environment.NewLine);
//                Thread.Sleep(50);
//            }// End of while loop
        } // End of Main function

        public class ServerWorkThread
        {
            public Socket mySocket;

            public ServerWorkThread()
            {
                IPEndPoint objEnpoint = new IPEndPoint(0, 20020);
                mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mySocket.Bind(objEnpoint);
                mySocket.Listen(100);
            }

            public void HandleConnection(Socket iIncomingSocket)
            {
                Thread worker = new Thread(this.RecieveAndSend);
                worker.Start(iIncomingSocket);
                worker.Join();
            }

            public void RecieveAndSend(object iIncoming)
            {
                Socket objSocket = (Socket) iIncoming;
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
                            if (File.Exists(MainClass.ftpPath + command))
                            {
                                Console.WriteLine("Let it play, let is play, let it play");
                                Console.WriteLine(command);
                                Process proc = new Process();
                                proc.EnableRaisingEvents = false;
                                if (!IsLinux)
                                {
                                    proc.StartInfo.FileName = @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe";
                                    proc.StartInfo.Arguments = "\"file:///" + MainClass.ftpPath + command + "\"";
                                }
                                else
                                {
                                    proc.StartInfo.FileName = "mplayer";
                                    proc.StartInfo.Arguments = "\"" + MainClass.ftpPath + command + "\"";
                                }
                                proc.Start();
                                strSend = "played";
                            }
                            else
                            {
                                strSend = "NoFile";
                            }
                            break;
                        case "SYS":
                            Console.WriteLine("System command");
                            break;

                        case "LED":

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