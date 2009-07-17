using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace HPPNet
{
    class AsyncUserToken
    {
        private Socket _socket;
        public Socket Socket
        {
            get { return _socket; }
            set { _socket = value; }
        }
    }
}
