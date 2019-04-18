using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        private static Socket serverSocket;
        private static List<Socket> clients;
        private static byte[] buffer = new byte[1024];

        public Form1()
        {
            InitializeComponent();
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clients = new List<Socket>();
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
            buttonStart.Enabled = false;
            textBoxPort.Enabled = false;

            backgroundWorkerStatus.RunWorkerAsync();
            StartServer(port);
        }

        private void StartServer(int port)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
            serverSocket.Bind(ep);
            serverSocket.Listen(0);

            try
            {
                serverSocket.BeginAccept(new AsyncCallback(AcceptClients), null);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Server error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AcceptClients(IAsyncResult ar)
        {
            backgroundWorkerStatus.RunWorkerAsync();
            try
            {

                Socket client = serverSocket.EndAccept(ar);
                clients.Add(client);
                serverSocket.BeginAccept(new AsyncCallback(AcceptClients), null);


                client.BeginReceive(buffer, 0,buffer.Length, SocketFlags.None,new AsyncCallback(DataRecieve), client);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void DataRecieve(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndReceive(ar);
                foreach(Socket s in clients)
                {
                    s.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(DataSend), s);
                }
                //string result = System.Text.Encoding.UTF8.GetString(buffer);
                //MessageBox.Show(result);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(DataRecieve), socket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        private void DataSend(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndSend(ar);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Errrrrror", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void backgroundWorkerStatus_DoWork(object sender, DoWorkEventArgs e)
        {
            labelStatus.Invoke((MethodInvoker)delegate {
                labelStatus.Text = "Server running - " + clients.Count() + " clients connected.";
            });
        }


    }
}
