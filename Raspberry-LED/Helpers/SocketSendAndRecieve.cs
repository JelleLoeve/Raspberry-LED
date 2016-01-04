using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;

namespace Raspberry_LED.Helpers
{
    public class SocketSendAndRecieve
    {
        private Socket pSocket;
        private string command;
        private volatile string recievedData;
        public SocketSendAndRecieve(string CommandType, string Command)
        {
            IPEndPoint objEnpoint = new IPEndPoint(IPAddress.Parse("192.168.0.100"), 20020);
            pSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            pSocket.Connect(objEnpoint);
            command = CommandType+":"+Command;
        }

        public void SendCommand()
        {
            Thread worker = new Thread(this.Send);
            worker.Start(pSocket);

        }

        public void Send(object iSending)
        {
            Socket objSocket = (Socket)iSending;
            objSocket.Send(System.Text.Encoding.UTF8.GetBytes(command));
            Debug.WriteLine(command);
            byte[] bytes = new byte[1024];
            int bytesRecieved = objSocket.Receive(bytes);
            recievedData = System.Text.Encoding.UTF8.GetString(bytes, 0, bytesRecieved);
            Debug.WriteLine(recievedData);
            objSocket.Close();
        }

        public string GetRecievedData()
        {
            return recievedData;
        }

    }
}