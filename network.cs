using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace csharp_server
{
    enum packets : byte
    {
        Disconnect, Name, Password, Message, Join, Leave
    }

    class _network
    {

        // returns a socket after establishing connection to a listen socket
        public static Socket joinListen(string ip, int port)
        {
            Socket connection = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            IPAddress hostIP = (Dns.GetHostEntry(ip)).AddressList[0];
            IPEndPoint ep = new IPEndPoint(hostIP, port);

            connection.Connect(ep);

            return connection;
        }


        // sends data to a socket
        public static int sendData(Socket destination, byte[] data)
        {
            int i = destination.Send(data);
            return i;
        }

        // receives data on the specified socket up to limit
        public static byte[] rcvData(Socket lstn, int limit)
        {
            byte[] buffer = new byte[limit];
            int size = lstn.Receive(buffer);

            if (size == 0) throw new Exception("Disconnected");

            byte[] data = new byte[size];
            for (int i = 0; i < size; ++i)
            {
                data[i] = buffer[i];
            }
            return data;
        }


        // receives a byte packet from a socket
        public static byte route(Socket lstn)
        {
            byte[] buffer = new byte[1];
            int packetLength = lstn.Receive(buffer);
            if (packetLength == 0) // someone must have disconnected
            {
                return (byte)packets.Disconnect;
            }
            else
            { // return the byte the client sent us to the caller
                return buffer[0];
            }
        }


        // closes socket activity
        public static void killSocket(Socket sock) 
        {
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }
    }
}
