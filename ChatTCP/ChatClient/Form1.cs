using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        private static Socket ServerSocket;
        private static List<TcpClient> Clients;
        private static byte[] buffer = new byte[1024];
        public Form1()
        {
            InitializeComponent();
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Clients = new List<TcpClient>();

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            int port = 0;
            try
            {
                port = Convert.ToInt32(textBoxPort.Text);
                if(port <0 || port > 65535)
                {
                    MessageBox.Show("Port should be in range beetwen 0 and 65535");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Enter the proper port");
                return;
            }
            StartServer();
        }
        private static void StartServer()
        {

        }
    }
}
