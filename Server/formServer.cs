using System;
using System.Threading;
using System.Windows.Forms;
using Server.Operators;

namespace Server
{
    public partial class Server : Form
    {
        ServerOperator _srv = new ServerOperator();

        public Server()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _srv.UserAddedInfoEvent += _srv_UserAddedEvent;
            _srv.UserRemoveEvent += _srv_UserRemoveEvent;
            _srv.UpdateGUIEvent += _srv_UpdateGUIEvent;
            _srv.StartServer();
        }

        private void btnServerStop_Click(object sender, EventArgs e)
        {
            _srv.Stop();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.ScrollToCaret();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void _srv_UpdateGUIEvent(string data)
        {
            // To Write the Received data
            this.Invoke((MethodInvoker)delegate
            {
                textBox1.AppendText($"{DateTime.Now.ToString("[dd MMMM yyyy] HH:mm:ss")} {Environment.NewLine}>> {data}{Environment.NewLine}{Environment.NewLine}");
            });
        }

        private void _srv_UserAddedEvent(string userName)
        {
            usersList.BeginInvoke(new ThreadStart(delegate
            {
                usersList.Items.Add(userName);
            }));
        }

        private void _srv_UserRemoveEvent(string userName)
        {
            usersList.BeginInvoke(new ThreadStart(delegate
            {
                usersList.Items.Remove(userName);
            }));
        }
    }
}
