using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Server : Form
    {
        TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
        TcpClient client;
        Dictionary<string, TcpClient> clientList = new Dictionary<string, TcpClient>();
        CancellationTokenSource cancellation = new CancellationTokenSource();
        List<string> chat = new List<string>();
        bool isLogin = false;

        public Server()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //resets the token when the server restarts
            cancellation = new CancellationTokenSource(); 
            startServer();
        }

        public void UpdateUI(String m)
        {
            // To Write the Received data
            this.Invoke((MethodInvoker)delegate 
            {
                textBox1.AppendText($"{DateTime.Now.ToString("[dd MMMM yyyy] HH:mm:ss")} {Environment.NewLine}>> {m}{Environment.NewLine}{Environment.NewLine}");
            });
        }

        public async void startServer()
        {
            listener.Start();
            UpdateUI("Server Started at " + listener.LocalEndpoint);
            UpdateUI("Waiting for Clients");
            try
            {
                int counter = 0;
                while (true)
                {
                    counter++;
                    //client = await listener.AcceptTcpClientAsync();
                    client = await Task.Run(() => listener.AcceptTcpClientAsync(), cancellation.Token);

                    /* get username */
                    byte[] creds = new byte[1000];
                    NetworkStream stream = client.GetStream(); 
                    stream.Read(creds, 0, creds.Length);

                    var parts = (List<string>)ByteArrayToObject(creds);
                    var userName = parts[0];
                    var pwd = parts[1];

                    if (isLogin = Auth.Login(userName, pwd))
                    {
                        /* add to dictionary, listbox and send userList  */
                        clientList.Add(userName, client);
                        listBox1.Items.Add(userName);
                        UpdateUI("Connected to user " + userName + " - " + client.Client.RemoteEndPoint);
                        announce(userName + " Joined ", userName, false);

                        await Task.Delay(1000).ContinueWith(t => sendUsersList());


                        var c = new Thread(() => ServerReceive(client, userName));
                        c.Start();
                    }
                    else
                    {
                        chat.Add("denied");
                        chat.Add($"{userName}:  Access Denied.");
                        var  broadcastBytes = ObjectToByteArray(chat);
                        stream.Write(broadcastBytes, 0, broadcastBytes.Length);
                        stream.Flush();
                        chat.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                listener.Stop();
            }

        }

        public void announce(string msg, string uName, bool flag)
        {
            try
            {
                foreach (var Item in clientList)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)Item.Value;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    Byte[] broadcastBytes = null;

                    if (flag)
                    {
                        switch (msg)
                        {
                            case "DATE":
                                chat.Add($" Command: {msg}{Environment.NewLine} >> Response: {DateTime.Now.ToString("dd MMMM yyyy")}");
                                break;

                            case "TIME":
                                chat.Add($" Command: {msg}{Environment.NewLine} >> Response: {DateTime.Now.ToString("HH:mm:ss")}");
                                break;

                            default:
                                chat.Add("chat");
                                chat.Add(uName + " says : " + msg);
                                break;
                        }

                        broadcastBytes = ObjectToByteArray(chat);
                    }
                    else
                    {
                        chat.Add("chat");
                        chat.Add(msg);
                        broadcastBytes = ObjectToByteArray(chat);
                    }

                    broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    broadcastStream.Flush();
                    chat.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        public byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                formatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        public void ServerReceive(TcpClient client, String userName)
        {
            byte[] data = new byte[1000];
            while (true)
            {
                try
                {
                    NetworkStream stream = client.GetStream(); 
                    stream.Read(data, 0, data.Length); //Receives Data 

                    List<string> parts = (List<string>)ByteArrayToObject(data);

                    switch (parts[0])
                    {
                        case "chat":
                            this.Invoke((MethodInvoker)delegate // To Write the Received data
                            {
                                textBox1.Text += userName + ": " + parts[1] + Environment.NewLine;
                            });
                            announce(parts[1], userName, true);
                            break;

                        case "some case":
                            break;
                    }

                    parts.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                    UpdateUI("Client Disconnected: " + userName);
                    announce("Client Disconnected: " + userName + "$", userName, false);
                    clientList.Remove(userName);

                    this.Invoke((MethodInvoker)delegate
                    {
                        listBox1.Items.Remove(userName);
                    });
                    sendUsersList();
                    break;
                }
            }
        }

        private void btnServerStop_Click(object sender, EventArgs e)
        {
            try
            {
                listener.Stop();
                UpdateUI("Server Stopped");
                foreach (var Item in clientList)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)Item.Value;
                    broadcastSocket.Close();
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }

            //cancellation.Cancel();
            //client.Close();   
        }

        public void sendUsersList()
        {
            try
            {
                byte[] userList = new byte[1024];
                string[] clist = listBox1.Items.OfType<string>().ToArray();
                List<string> users = new List<string>();

                users.Add("userList");
                foreach (String name in clist)
                {
                    users.Add(name);
                }
                userList = ObjectToByteArray(users);

                foreach (var Item in clientList)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)Item.Value;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    broadcastStream.Write(userList, 0, userList.Length);
                    broadcastStream.Flush();
                    users.Clear();
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.ScrollToCaret();
        }
    }
}
