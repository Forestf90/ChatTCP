using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Client
    {
        public Socket socket { get; set; }
        public string Nick { get; set; }
        public byte[] buffer { get; set; }
        public string GetNick()
        {
            return "[" + Nick + "]:";
        }
    }
}
