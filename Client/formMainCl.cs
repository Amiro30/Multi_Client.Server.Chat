using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class formMainCl : Form
    {
        public TcpClient clientSocket;
        public NetworkStream serverStream = default(NetworkStream);
        string readData = null;
        Thread clientThread;
        String _name = null;
        String _pass = null;
        List<string> chat = new List<string>();

        public void setCredentials(string login, string pass)
        {
            _name = login;
            _pass = pass;
        }

        public formMainCl()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (!input.Text.Equals(""))
                {
                    chat.Add("chat");
                    chat.Add(input.Text);
                    byte[] outStream = ObjectToByteArray(chat);

                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();
                    input.Text = "";
                    chat.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                btnConnect.Enabled = true;
            }

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            clientSocket = new TcpClient();
            try
            {
                clientSocket.Connect("127.0.0.1", 5000);
                readData = "Connected to Server ";
                msg();

                serverStream = clientSocket.GetStream();

                //byte[] outStream = Encoding.ASCII.GetBytes(_name + "$" + _pass + "*");
                chat.Add(_name);
                chat.Add(_pass);
                byte[] outStream = ObjectToByteArray(chat);


                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                btnConnect.Enabled = false;


                clientThread = new Thread(getMessage);
                clientThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Server Not Started");
            }
        }

        public void getUsers(List<string> parts)
        {
            this.Invoke((MethodInvoker)delegate
            {
                listBox1.Items.Clear();
                for (int i = 1; i < parts.Count; i++)
                {
                    listBox1.Items.Add(parts[i]);

                }
            });
        }

        private void getMessage()
        {
            try
            {
                while (true)
                {
                    serverStream = clientSocket.GetStream();
                    byte[] inStream = new byte[10025];
                    serverStream.Read(inStream, 0, inStream.Length);
                    List<string> parts = null;

                    if (!SocketConnected(clientSocket))
                    {
                        MessageBox.Show("You've been Disconnected");
                        clientThread.Abort();
                        clientSocket.Close();
                        btnConnect.Enabled = true;
                    }

                    parts = (List<string>)ByteArrayToObject(inStream);
                    switch (parts[0])
                    {
                        case "userList":
                            getUsers(parts);
                            break;

                        case "chat":
                            readData = "" + parts[1];
                            msg();
                            break; 

                        case "ACHTUNG!":
                            readData = "" + parts[1];
                            msg();

                            Application.Restart();
                            Environment.Exit(0);
                            break;

                        default:
                            readData = "" + parts[0];
                            msg();
                            break;

                    }

                    if (readData[0].Equals('\0'))
                    {
                        readData = "Reconnect Again";
                        msg();

                        this.Invoke((MethodInvoker)delegate // To Write the Received data
                        {
                            btnConnect.Enabled = true;
                        });

                        clientThread.Abort();
                        clientSocket.Close();
                        break;
                    }
                    chat.Clear();
                }
            }
            catch (Exception e)
            {
                clientThread.Abort();
                clientSocket.Close();
                btnConnect.Enabled = true;
                Console.WriteLine(e);
            }
        }

        private void msg()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(msg));
            else
                history.Text = history.Text + Environment.NewLine + " >> " + readData;
        }

        private void formMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Do you want to exit? ", "Exit", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                try
                {
                    clientThread.Abort();
                    clientSocket.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }

                Application.ExitThread();
            }
            else if (dialog == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        public byte[] ObjectToByteArray(object _Object)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, _Object);
                return stream.ToArray();
            }
        }

        public Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        //check whether client is connected server
        bool SocketConnected(TcpClient s) 
        {
            bool flag = false;
            try
            {
                bool part1 = s.Client.Poll(10, SelectMode.SelectRead);
                bool part2 = (s.Available == 0);
                if (part1 && part2)
                {
                    indicator.BackColor = Color.Red;
                    this.Invoke((MethodInvoker)delegate // cross threads
                    {
                        btnConnect.Enabled = true;
                    });
                    flag = false;
                }
                else
                {
                    indicator.BackColor = Color.Green;
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return flag;
        }

        private void btnClr_Click(object sender, EventArgs e)
        {
            history.Clear();
        }

        private void history_TextChanged(object sender, EventArgs e)
        {
            history.SelectionStart = history.TextLength;
            history.ScrollToCaret();
        }
    }
}
