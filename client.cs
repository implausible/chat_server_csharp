using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace csharp_server
{
    class _client
    {
        private Socket mSocket;
        private string mName;
        private uint mID;

        public _client(Socket pSocket, string pName, uint pID)
        {
            mSocket = pSocket;
            mName = pName;
            mID = pID;
        }

        public string getName()
        {
            return mName;
        }

        public byte getNextPacket()
        {
            byte packet = _network.route(mSocket);
            return packet;
        }

        public string getMessage()
        {
            try
            {
                // first we need to discover the length of the packet being sent
                // This chat server runs over a stream connection, so basically
                // if we, the server, expect a static length message
                // then we might be stuck in the same receive even though we're sending new
                // packets.
                byte[] lengthInBytes = _network.rcvData(mSocket, 4);
                int length = BitConverter.ToInt32(lengthInBytes, 0);

                if (length > _config.getMaxMsgSize()) length = (int)_config.getMaxMsgSize(); // no super large messages allowed
                if (length < 0) length = 1;
                // the packet handler will decide to ignore all nonpacket related messages
                // so in the next line we use the BitConverter library to discover the length of the message
                byte[] data = _network.rcvData(mSocket, length);

                return new string(Encoding.UTF8.GetChars(data));
            }
            catch (Exception mErr)
            {
                throw mErr;
            }
        }

        public string getNewName()
        {
            try
            {
                byte[] lengthInBytes = _network.rcvData(mSocket, 4);
                int length = BitConverter.ToInt32(lengthInBytes, 0);

                if (length > _config.getMaxNameSize()) length = (int)_config.getMaxNameSize();
                if (length < 0) length = 1;

                // so in the next line we use the BitConverter library to discover the length of the message
                byte[] data = _network.rcvData(mSocket, length);
                return new string(Encoding.UTF8.GetChars(data));
            }
            catch (Exception mErr)
            {
                throw mErr;
            }
        }

        public bool isConnected()
        {
            return mSocket.Connected;
        }

        public void sendDisconnect()
        {
            byte[] packet = new byte[1];
            packet[0] = (byte)packets.Disconnect;
            _network.sendData(mSocket, packet);
        }

        public void kill()
        {
            _network.killSocket(mSocket);
        }

        public void setName(string pName)
        {
            mName = pName;
        }
    }
}
