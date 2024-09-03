using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AvaloniaMessenger.Models;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using ReactiveUI;
using System.Reactive;
using System.Text.Json.Nodes;
using System.Reflection;
using System.Net.NetworkInformation;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace AvaloniaMessenger.Controllers
{
    internal class WebSocketController
    {
        public ClientWebSocket WebSocket = new();
        public ReactiveCommand<Message, Unit> LoadNewMessagesCommand;
        public ReactiveCommand<Chat, Unit> LoadNewChatCommand;
        public ReactiveCommand<ClientWebSocket, Unit> ConnectionFailedCommand; 
        public delegate void NewMessage(Message message);
        public event NewMessage? NotifyNewMessage;
        private string connectionString;
        public WebSocketController(string connectionString)
        {
            this.connectionString = connectionString;
            SetWebSocket(connectionString);
        }
        private async Task SetWebSocket(string connectionString)
        {
            await WebSocket.ConnectAsync(new Uri(connectionString), CancellationToken.None);

            await Task.Run(() => ReceiveMessage());
        }
        private async void ReconnectToServer()
        {
            while(WebSocket.State != WebSocketState.Open)
            {
                WebSocket = new();

                if (TestConnectionToServer())
                {
                    try
                    {
                        await SetWebSocket(connectionString);
                    }
                    catch
                    {
                        Thread.Sleep(5000);
                    }
                    
                }
            }
        }
        private bool TestConnectionToServer()
        {
            var uri = new Uri(connectionString);

            using (TcpClient client = new TcpClient())
            {
                try
                {
                    client.Connect(uri.Host, uri.Port);
                    return true;
                }
                catch (SocketException)
                {
                    return false;
                }
            }

        }
        private void ReceiveMessage()
        {
            var buffer = new byte[1024 * 4];

            while (WebSocket.State == WebSocketState.Open)
            {
                try
                {
                    var receiveResult = WebSocket.ReceiveAsync(
                   new ArraySegment<byte>(buffer, 0, buffer.Length),
                   CancellationToken.None).Result;
                }
                catch
                {
                    Task.Run(() => ConnectionFailedCommand.Execute(WebSocket).Subscribe());
                    try
                    {
                        ReconnectToServer();
                    }
                    catch
                    {

                    }
                    continue;
                }
                
                string messageJson = Encoding.UTF8.GetString(buffer).Trim('\0');

                var jsonObj = JsonObject.Parse(messageJson);

                var methods = GetType()
                    .GetMethods().Where(i => i.GetCustomAttribute<TableNotification>() != null);

                var method = methods.FirstOrDefault(i => i.GetCustomAttribute<TableNotification>().TableName == jsonObj["table"].ToString());


                if (method == null)
                    throw new Exception("Method to parse a payload was not found");

                method.Invoke(this, new object[]{ messageJson });

                Array.Clear(buffer);
            }
        }
        [TableNotification("Messages")]
        public void ProccessMessage(string notificationJson)
        {
            var notification = JsonConvert.DeserializeObject<DbNotification<Message>>(notificationJson);

            if (notification == null)
                return;

            if (LoadNewMessagesCommand != null)
                LoadNewMessagesCommand.Execute(notification.data).Subscribe();
        }
        [TableNotification("Chats")]
        public void ProccessChat(string notificationJson)
        {
            var notification = JsonConvert.DeserializeObject<DbNotification<Chat>>(notificationJson);

            if (notification == null)
                return;

            if (LoadNewChatCommand != null)
                LoadNewChatCommand.Execute(notification.data).Subscribe();
        }

        public async Task SendMessage(Message message)
        {
            var sendBuffer = new byte[1024 * 4];
            if (WebSocket.State == WebSocketState.Open)
            {
                sendBuffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                await WebSocket.SendAsync(sendBuffer,
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

                Array.Clear(sendBuffer);
            }
        }
    }
    class TableNotification : Attribute
    {
        public string TableName;
        public TableNotification(string tableName)
        {
            TableName = tableName;
        }
    }
}
