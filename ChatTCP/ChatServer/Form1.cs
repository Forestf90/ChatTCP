using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        public string userName;
        public Socket socket;
        public byte[] buffer = new byte[1024];
        public Form1()
        {
            InitializeComponent();

            userName = radioButtonAn.Text;
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(textBoxName.Text))
            {
                userName = textBoxName.Text;
            }
        }

        private void textBoxName_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textBoxName.Text))
            {
                //userName = "Anonymous";
                radioButtonAn.Checked=true;
            }
        }

        private void radioButtonNa_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonNa.Checked == true)
            {
                textBoxName.Enabled = true;
            }
            else
            {
                textBoxName.Enabled = false;
                textBoxName.Text = null;
                userName = "Anonymous";
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {

            int port = 0;
            try
            {
                port = Convert.ToInt32(textBoxPort.Text);
                if (port < 0 || port > 65535)
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

            IPAddress ip;

            try
            {
                ip = IPAddress.Parse(textBoxIP.Text);
                IPEndPoint ep = new IPEndPoint(ip, port);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.BeginConnect(ep, new AsyncCallback(ConnectToServer), null);
            }
            catch
            {
                MessageBox.Show("Enter the proper IP");
                return;
            }

        }


        private void ConnectToServer(IAsyncResult ar)
        {
            try
            {
                socket.EndConnect(ar);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(DataRecieve), null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void DataRecieve(IAsyncResult ar)
        {
            try
            {
                //Socket socket = (Socket)ar.AsyncState;
                socket.EndReceive(ar);
                string receiveMassage = System.Text.Encoding.UTF8.GetString(buffer);
                richTextBoxChat.Text += receiveMassage + System.Environment.NewLine;
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(DataRecieve), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            buffer = Encoding.UTF8.GetBytes(textBoxSend.Text);

            socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(DataSend), null);
        }

        private void DataSend(IAsyncResult ar)
        {
            try
            {
                socket.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
