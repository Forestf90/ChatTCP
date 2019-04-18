using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        public string userName;
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
                userName = "Anonymous";
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            richTextBoxChat.Text = userName;
        }
    }
}
