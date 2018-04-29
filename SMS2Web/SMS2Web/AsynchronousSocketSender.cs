using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SMS2Web
{
    // State object for reading client data asynchronously  
    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketSender
    {
        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        protected static IPEndPoint endPoint;
        protected static Socket sender;

        public static void SetAddress(string address)
        {
            string[] addressSplit2 = address.Split(':');
            string[] addressSplit4 = addressSplit2[0].Split('.');
            if (addressSplit2.Length != 2)
            {
                throw new Exception();
            }

            string trueAddress = addressSplit2[0];
            int port = 0;

            if (!Int32.TryParse(addressSplit2[1], out port))
            {
                throw new Exception();
            }

            Byte[] byteAddress = new Byte[addressSplit4.Length];

            for (int i = 0; i < byteAddress.Length; i++)
            {
                byteAddress[i] = (byte)Int16.Parse(addressSplit4[i]);
            }

            IPAddress ipAddress = new IPAddress(byteAddress);
            endPoint = new IPEndPoint(ipAddress, port);
        }

        

        public static void Send(String data)
        {
            sender = new Socket(SocketType.Stream, ProtocolType.Tcp);

            sender.Connect(endPoint);
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.Unicode.GetBytes(data);

            // Begin sending the data to the remote device.  
            sender.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), sender);
        }

        protected static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
            }
        }

    }
}