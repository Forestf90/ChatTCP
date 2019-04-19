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
        private static List<Client> clients;
        private static byte[] buffer = new byte[8192];

        public Form1()
        {
            InitializeComponent();
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clients = new List<Client>();
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

            StartServer(port);
        }

        private void StartServer(int port)
        {
            
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
            serverSocket.Bind(ep);
            serverSocket.Listen(0);
            LabelUpdate();
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
            //LabelUpdate();
            try
            {

                Socket client = serverSocket.EndAccept(ar);
                Client temp = new Client();
                temp.socket = client;
                temp.buffer = new byte[8192];
                clients.Add(temp);
                LabelUpdate();
                serverSocket.BeginAccept(new AsyncCallback(AcceptClients), null);


                client.BeginReceive(temp.buffer, 0, temp.buffer.Length, SocketFlags.None,new AsyncCallback(DataRecieve), temp);
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
                Client cl = (Client)ar.AsyncState;
                Socket socket = cl.socket;
                socket.EndReceive(ar);
                bool part1 = socket.Poll(1000, SelectMode.SelectRead);
                bool part2 = (socket.Available == 0);
                if (part1 && part2)
                {
                    string receiveMassage = cl.GetNick() + " left chat";
                    clients.Remove(cl);
                    cl.socket.Shutdown(SocketShutdown.Both);
                    LabelUpdate();
                    byte[] bufferTemp = System.Text.Encoding.UTF8.GetBytes(Environment.NewLine + receiveMassage);
                    foreach (Client c in clients)
                    {
                        c.socket.BeginSend(bufferTemp, 0, bufferTemp.Length, SocketFlags.None, new AsyncCallback(DataSend), c.socket);
                    }
                    return;
                }


                if (String.IsNullOrWhiteSpace(System.Text.Encoding.UTF8.GetString(cl.buffer).TrimEnd('\0'))){ }
                else if (String.IsNullOrWhiteSpace(cl.Nick))
                {
                    string clientName = System.Text.Encoding.UTF8.GetString(cl.buffer);
                    clientName= clientName.TrimEnd('\0');
                    cl.Nick = clientName;
                    clientName = "<<< " + cl.Nick + " join chat >>>".Replace(Environment.NewLine, "");
                    byte[] bufferTemp = System.Text.Encoding.UTF8.GetBytes(Environment.NewLine+clientName);
                    foreach (Client c in clients)
                    {
                        c.socket.BeginSend(bufferTemp, 0, bufferTemp.Length, SocketFlags.None, new AsyncCallback(DataSend), c.socket);
                    }
                }
                else
                {
                    string receiveMassage = cl.GetNick() + System.Text.Encoding.UTF8.GetString(cl.buffer);
                    byte[] bufferTemp = System.Text.Encoding.UTF8.GetBytes(Environment.NewLine+receiveMassage);
                    foreach (Client c in clients)
                    {
                        c.socket.BeginSend(bufferTemp, 0, bufferTemp.Length, SocketFlags.None, new AsyncCallback(DataSend), c.socket);
                    }
                }
                cl.buffer = new byte[8192];
                socket.BeginReceive(cl.buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(DataRecieve), cl);
            }
            catch (SocketException ex)
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
                MessageBox.Show(ex.Message, "Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LabelUpdate()
        {
            labelStatus.Invoke((MethodInvoker)delegate {
                labelStatus.Text = "Server running - " + clients.Count() + " clients connected.";
            });
        }
    }
}
