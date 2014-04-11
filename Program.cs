using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;



namespace csharp_server
{
    class Program
    {
        static void Main(string[] args)
        {
            _connectionHandler c = new _connectionHandler(27015);
            while (_writer.readLine() != "/quit") { }
            c.shutdown();
        }
    }

    class _writer
    {
        private static int lastWrite = Console.CursorTop;
        private static bool busy = false;

        public static string readLine()
        {
            string output = Console.ReadLine();
            lastWrite = Console.CursorTop;
            return output;
        }

        public static void writeLine(string msg, params Object[] args)
        {
            while (busy) { Thread.Sleep(10); }
            busy = true;
            int left = Console.CursorLeft,
                currentLine = Console.CursorTop,
                msgLineSpace;

            msg = string.Format(msg, args);

            // how many lines does msg need to write
            msgLineSpace = (int)Math.Ceiling(msg.Length / (double)Console.WindowWidth);
            
            if (left == 0)
            {
                Console.WriteLine(msg);
                lastWrite = Console.CursorTop;
            }
            else
            {
                int linesInBuffer = 1 + currentLine - lastWrite;
                Console.MoveBufferArea(0, lastWrite, Console.WindowWidth, linesInBuffer, 0, lastWrite + msgLineSpace);
                Console.SetCursorPosition(0, lastWrite);
                Console.WriteLine(msg);
                lastWrite = Console.CursorTop;
                Console.SetCursorPosition(left, currentLine + 1);
            }
            busy = false;
        }

        public static void writeLine(char[] msg)
        {
            writeLine(new string(msg));
        }
    }

    class _config
    {
        private static uint mMaxMsgSize;
        private static uint mMaxNameSize;

        public static void setMaxMsgSize(uint pMaxMsgSize) {
            mMaxMsgSize = pMaxMsgSize;
        }

        public static void setMaxNameSize(uint pMaxNameSize)
        {
            mMaxNameSize = pMaxNameSize;
        }

        public static uint getMaxMsgSize()
        {
            return mMaxMsgSize;
        }

        public static uint getMaxNameSize()
        {
            return mMaxNameSize;
        }
    }
}
