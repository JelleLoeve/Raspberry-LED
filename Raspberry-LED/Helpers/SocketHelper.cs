using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace Raspberry_LED.Helpers
{
	public class SocketHelper
	{
	    public static int ServerSocketPort = 12345;
        private Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private IPEndPoint ipendpoint;
        
        public SocketHelper(string ip)
        {
            ipendpoint = new IPEndPoint(IPAddress.Parse(ip), ServerSocketPort);
        }
        public bool connectToSocket()
        {
            
            try
            {
                socket.Connect(ipendpoint);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public void SendToServer(string dataToSend)
        {
            Reconnect();
            byte[] data = Encoding.ASCII.GetBytes(dataToSend);
            socket.Send(data);
            Console.WriteLine("Data send! Data: \"" + dataToSend + "\"");
            socket.Close();
            Thread.Sleep(50);
        }

        public void Close()
        {
            socket.Close();
        }

        public void Reconnect()
        {
            socket.Close();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                socket.Connect(ipendpoint);
            }
            catch (Exception e)
            {
               Console.WriteLine("Couldn't connect to the server.\nCheck if internet still works or if the server isn't down");
            }
            
        }
	}
}

