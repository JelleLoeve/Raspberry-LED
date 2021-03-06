﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Raspberry_LED_Client
{
    public class ServerWorkThread
    {
        public static int ServerPort = 20020;
        private static byte[] Buffer { get; set; }
        public static string ftpPath = !Helpers.IsLinux ? "C:/inetpub/ftproot/" : "/home/pi/music/Raspberry-LED/";
        public Socket mySocket;

        public ServerWorkThread()
        {
            IPEndPoint objEnpoint = new IPEndPoint(0, ServerPort);
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mySocket.Bind(objEnpoint);
            Console.WriteLine("Awaiting Data");
            mySocket.Listen(100);
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
            string strSend = String.Empty;
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
                            if (!Helpers.IsLinux)
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
