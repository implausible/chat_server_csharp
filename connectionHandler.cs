using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace csharp_server
{
    class _connectionHandler
    {
        // if mClients is organized as a dictionary
        // we can just lookup clients by id.
        private Dictionary<uint, _client> mClients;

        // list of used names
        private List<string> mNames;

        // ids that have been discarded
        private Stack<uint> mUsedIDs;

        // the next assignable id
        private uint mStartIDs;

        // stack containing all threads used by _connectionHandler
        // mainly for termination of threads and clean up
        private Stack<Thread> mThreads;

        // These are necessary for accepting incoming connections
        private TcpListener mListener;

        private int mPort;

        private bool mActive;

        public _connectionHandler(int pPort)
        {
            mClients = new Dictionary<UInt32, _client>();
            mNames = new List<string>();
            mUsedIDs = new Stack<uint>();
            mStartIDs = 0;
            mThreads = new Stack<Thread>();
            mPort = pPort;
            mActive = true;

            IPAddress hostIP = (Dns.GetHostEntry("127.0.0.1")).AddressList[0];
            IPEndPoint ep = new IPEndPoint(hostIP, mPort);
            mListener = new TcpListener(ep);
            mListener.Start();

            Thread mClientLoop = new Thread(joinLoop);
            mClientLoop.Start();
            mThreads.Push(mClientLoop);
            mClientLoop = null;
        }

        public void shutdown()
        {
            _client cl;
            int sizeOfClients = mClients.Count;

            // tell the server to stop looping for new connections
            mActive = false;

            // send a dummy client to the server
            dummyClientDisconnect();
            mListener.Stop();

            while (mThreads.Count > 0)
            {
                Thread s = mThreads.Pop();
                s.Abort();
            }
            for (uint i = 0; i < sizeOfClients; ++i)
            {
                if (!mUsedIDs.Contains(i) && mClients.TryGetValue(i, out cl))
                {
                    // delete the client associated with the ID
                    cl.sendDisconnect();
                    cl.kill();
                    mClients.Remove(i);
                }
            }
        }

        // joinLoop is responsible for adding clients to the server and managing their unique IDs
        // joinLoop will start a new thread for every client that joins the server.
        private void joinLoop()
        {
            UInt32 ID;
            while (mActive)
            {
                Socket nClient = mListener.AcceptSocket();

                // if the serve has shut down
                // don't add the new client
                // it's a fake client
                if (!mActive) return;

                if (mUsedIDs.Count == 0)
                {
                    ID = mStartIDs;
                    ++mStartIDs;
                }
                else
                {
                    ID = mUsedIDs.Pop();
                }
                mClients.Add(ID, new _client(nClient, "Anon_" + ID, ID));
                mNames.Add("Anon_" + ID);
                _writer.writeLine("A client has joined with ID: {0}", ID);
                Thread cl = new Thread(clientPacketLoop);
                cl.Start(ID);
                mThreads.Push(cl);
                cl = null;
                nClient = null;
            }
        }

        private void dummyClientDisconnect()
        {
            byte[] packet = new byte[1];
            Socket dummy = _network.joinListen("127.0.0.1", mPort);
            packet[0] = (byte)packets.Disconnect;
            _network.sendData(dummy, packet);
            _network.killSocket(dummy);
            dummy = null;
        }

        private void clientPacketLoop(object param)
        {
            uint pID = (uint)param;
            _client cl;
            while (mClients.TryGetValue(pID, out cl))
            {
                byte packet = cl.getNextPacket();
                try
                {
                    switch (packet)
                    {
                        case (byte)packets.Disconnect:
                            removeClient(pID);
                            break;
                        case (byte)packets.Message:
                            _writer.writeLine("{0}: {1}", cl.getName(), cl.getMessage());
                            break;
                        case (byte)packets.Name:
                            string name = cl.getNewName();
                            if (!mNames.Contains(name))
                            {
                                mNames.Add(name);
                                cl.setName(name);
                            }
                            break;
                    }
                }
                catch (Exception mErr)
                {
                    if (mErr.Message == "Disconnected")
                    {
                        removeClient(pID);
                    }
                }
                cl = null;
            }
        }
        private void removeClient(uint pID)
        {
            _client cl;
            if (mClients.TryGetValue(pID, out cl))
            {
                _writer.writeLine("Client {0} has disconnected.", pID);
                mClients.Remove(pID);
                mUsedIDs.Push(pID);
                cl.kill();
                mNames.Remove(cl.getName());
            }
            cl = null;
        }
    }
}