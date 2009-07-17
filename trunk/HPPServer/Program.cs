using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HPPServer;
using HPPNet;
using System.Net;
using System.Net.Sockets;
using HPPUtil;
using HPPClientLibrary;
namespace HPPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            SSServer server = new SSServer(10,Int16.MaxValue);
            server.Init();
            server.Start(new IPEndPoint(IPAddress.Any, 9999));
        }
    }
}
