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
using AvaloniaMessenger.ViewModels;

namespace AvaloniaMessenger.Controllers
{
    internal class WebSocketController
    {
        public ClientWebSocket WebSocket = new();
        public WebSocketMessageReceiver WebSocketReceiver;

        public ReactiveCommand<Message, Unit> LoadNewMessagesCommand;
        public ReactiveCommand<ChatUser, Unit> LoadNewChatCommand;
        public ReactiveCommand<Chat, Unit> DeleteChatCommand;
        public ReactiveCommand<ClientWebSocket, Unit> ConnectionFailedCommand;

        private string connectionString;
        public WebSocketController(string connectionString)
        {
            this.connectionString = connectionString;
            WebSocketReceiver = new(this);
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

                var methods = WebSocketReceiver.GetType()
                    .GetMethods().Where(i => i.GetCustomAttribute<TableNotification>() != null);

                var method = methods.FirstOrDefault(i => 
                i.GetCustomAttribute<TableNotification>().TableName == jsonObj["table"].ToString() &&
                i.GetCustomAttribute<TableNotification>().Action == jsonObj["action"].ToString());


                if (method == null)
                {
                    Array.Clear(buffer);
                    continue; 
                }

                method.Invoke(WebSocketReceiver, new object[]{ messageJson });

                Array.Clear(buffer);
            }
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
}
