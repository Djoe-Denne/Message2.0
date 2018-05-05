using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static SMSReceiver.AsynchronousSocketListener;

namespace SMSReceiver
{
    // State object for reading client data asynchronously  
    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        private static int bufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();

        public static int BufferSize
        {
            get
            {
                return bufferSize;
            }
        }
    }

    public static class AsynchronousSocketListener
    {
        private static uint port = 11000;
        private static IPAddress address = null;
        private static bool close = false;
        private static Mutex mut = new Mutex();

        public static uint Port
        {
            get => port;
            set
            {
                if( value > 65535)
                {
                    throw new Exception();
                }
                port = value;
            }
        }

        public static IPAddress GetAddress()
        {
            if (address == null)
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress addressIt in ipHostInfo.AddressList)
                {
                    if (addressIt.GetAddressBytes().Length == 4)
                    {
                        address = addressIt;
                        break;
                    }
                }
            }/*
            if (address == null)
                throw new Exception("No network detected");*/
            
            return address;
        }

        public static void SetAddress(String addressStr)
        {

            string[] addressSplit = addressStr.Split('.');
            

            Byte[] byteAddress = new Byte[addressSplit.Length];

            for (int i = 0; i < byteAddress.Length; i++)
            {
                byteAddress[i] = (byte)Int16.Parse(addressSplit[i]);
            }

            address = new IPAddress(byteAddress);
        }

        private static bool Close
        {
            get
            {
                bool ret;
                mut.WaitOne();
                ret = close;
                mut.ReleaseMutex();
                return ret;
            }
            set
            {
                mut.WaitOne();
                close = value;
                mut.ReleaseMutex();
            }
        }

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public delegate void MessagePushingEvent(Byte[] buffer);

        private static MessagePushingEvent pmListener;

        public static void AddMessagePushingListener(MessagePushingEvent listener)
        {
            pmListener += listener;
        }

        public static void RemoveMessagePushingListener(MessagePushingEvent listener)
        {
            pmListener -= listener;
        }

        public static void StartListening()
        {
            Close = false;
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  

            if (GetAddress() == null)
            {
                return;
            }

            IPEndPoint localEndPoint = new IPEndPoint(GetAddress(), (int)Port);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(address.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            do
            {
                // Bind the socket to the local endpoint and listen for incoming connections.  
                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(100);
                    
                    while (!Close)
                    {
                        // Set the event to nonsignaled state.  
                        allDone.Reset();

                        // Start an asynchronous socket to listen for connections.  
                        Console.WriteLine("Waiting for a connection on : " + address + ":" + Port);
                        listener.BeginAccept(
                            new AsyncCallback(AcceptCallback),
                            listener);

                        // Wait until a connection is made before continuing.  
                        allDone.WaitOne();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            } while (!Close);

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void Stop()
        {
            Close = true;
            allDone.Set();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                String content = String.Empty;

                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();
                /*if (content.IndexOf("<EOF>") > -1)
                {*/
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    pmListener(state.buffer);
                
                /*
            }
            else
            {
                // Not all data received. Get more.  
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
            }*/
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}