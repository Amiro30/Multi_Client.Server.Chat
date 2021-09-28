using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Server.Models;


namespace Server.Operators
{
    class ServerOperator
    {
        public delegate void GetUserName(string userName);
        public delegate void UpdateGUI(string data);

        public event GetUserName UserAddedInfoEvent;
        public event GetUserName UserRemoveEvent;
        public event UpdateGUI UpdateGUIEvent;

        TcpClient client;
        TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);

        //resets the token when the server restarts
        CancellationTokenSource cancellation = new CancellationTokenSource();
        Helper sHelper = new Helper();
        ChatRepository dbRepository = new ChatRepository();
      

        Dictionary<string, TcpClient> clientList = new Dictionary<string, TcpClient>();
        List<string> chat = new List<string>();

        public async void StartServer()
        {
            if (dbRepository.IsDisposed())
            {
                dbRepository = new ChatRepository();
            }

            listener.Start();
            UpdateGUIEvent?.Invoke($"Server Started at {listener.LocalEndpoint}{Environment.NewLine} Waiting for Clients");

            try
            {
                while (true)
                {
                    client = await Task.Run(() => listener.AcceptTcpClientAsync(), cancellation.Token);

                    /* get username */
                    byte[] creds = new byte[1000];
                    NetworkStream stream = client.GetStream();
                    stream.Read(creds, 0, creds.Length);

                    var parts = sHelper.ByteArrayToObject(creds) as List<string>;                 
                    var userName = parts[0];
                    var pwd = parts[1];

                    if (Auth.Login(userName, pwd) && !dbRepository.UserExist(userName))
                    {
                        /* add to dictionary, listbox and send userList  */
                        clientList.Add(userName, client);

                        //Save user to DB
                        dbRepository.SaveUser(new User
                        {
                            Login = userName,
                            Password = pwd
                        });

                        //Update UI
                        UserAddedInfoEvent?.Invoke(userName);
                        UpdateGUIEvent?.Invoke("Connected to user " + userName + " - " + client.Client.RemoteEndPoint);

                        BroadCastMessage(userName + " Joined ", userName, false);

                        await Task.Delay(1000).ContinueWith(t => SendUsersList());

                        var c = new Thread(() => ServerReceiveData(client, userName));
                        c.Start();
                    }
                    else if(Auth.Login(userName, pwd) && dbRepository.UserExist(userName))
                    {
                        /* add to dictionary, listbox and send userList  */
                        clientList.Add(userName, client);
                        //Update UI
                        UserAddedInfoEvent?.Invoke(userName);
                        UpdateGUIEvent?.Invoke("Connected to user " + userName + " - " + client.Client.RemoteEndPoint);

                        BroadCastMessage(userName + " Joined ", userName, false);

                        await Task.Delay(1000).ContinueWith(t => SendUsersList());

                        var c = new Thread(() => ServerReceiveData(client, userName));
                        c.Start();
                    }
                    else
                    {
                        chat.Add("ACHTUNG!");
                        chat.Add($"{userName}:  Access Denied.");
                        var broadcastBytes = sHelper.ObjectToByteArray(chat);
                        stream.Write(broadcastBytes, 0, broadcastBytes.Length);
                        stream.Flush();
                        chat.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                listener.Stop();
            }

        }

        public void ServerReceiveData(TcpClient client, String userName)
        {
            byte[] data = new byte[1000];
            while (true)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    stream.Read(data, 0, data.Length); //Receives Data 

                    List<string> parts = sHelper.ByteArrayToObject(data) as List<string>; 

                    switch (parts[0])
                    {
                        case "chat":
                            UserAddedInfoEvent?.Invoke(userName + ": " + parts[1] + Environment.NewLine);

                            BroadCastMessage(parts[1], userName, true);
                            break;
                    }

                    parts.Clear();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    UpdateGUIEvent?.Invoke("Client Disconnected: " + userName);
                    BroadCastMessage("Client Disconnected: " + userName + "$", userName, false);
                    clientList.Remove(userName);

                    UserRemoveEvent?.Invoke(userName);
                    SendUsersList();
                    break;
                }
            }
        }

        public void SendUsersList()
        {
            try
            {
                byte[] userList = new byte[1024];
                userList = sHelper.ObjectToByteArray(clientList.Select(x => x.Key).ToList());

                foreach (var client in clientList)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)client.Value;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    broadcastStream.Write(userList, 0, userList.Length);
                    broadcastStream.Flush();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void BroadCastMessage(string msg, string uName, bool flag)
        {
            try
            {
                //broadcasting chat msgs to all clients
                foreach (var connectedClient in clientList)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)connectedClient.Value;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    Byte[] broadcastBytes = null;

                    if (flag)
                    {
                        if (connectedClient.Key == uName)
                        {
                            var user = dbRepository.GetUserId(uName);

                            //save msg to Db only for sender
                            dbRepository.SaveMessage(new Message
                            {
                                Msg = msg,
                                DateIn = DateTime.Now,
                                SenderId = user.UserId
                            });
                        }

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

                        broadcastBytes = sHelper.ObjectToByteArray(chat);
                    }
                    else
                    {
                        chat.Add("chat");
                        chat.Add(msg);
                        broadcastBytes = sHelper.ObjectToByteArray(chat);
                    }

                    broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    broadcastStream.Flush();
                    chat.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void Stop()
        {
            try
            {
                listener.Stop();
                UpdateGUIEvent?.Invoke("Server Stopped");

                foreach (var Item in clientList)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)Item.Value;
                    broadcastSocket.Close();
                }
                dbRepository.Dispose();
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
