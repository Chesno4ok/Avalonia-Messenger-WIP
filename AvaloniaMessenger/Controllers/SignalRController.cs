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
using Microsoft.AspNetCore.SignalR.Client;

namespace AvaloniaMessenger.Controllers
{
    internal class SignalRController
    {
        public ReactiveCommand<Message, Unit> LoadNewMessagesCommand;
        public ReactiveCommand<Chat, Unit> LoadNewChatCommand;
        public ReactiveCommand<Chat, Unit> DeleteChatCommand;
        public ReactiveCommand<Unit, Unit> ConnectionFailedCommand;

        HubConnection connection;

        private string connectionString;
        public  SignalRController(string connectionString, string token)
        {
            this.connectionString = connectionString;

            connection = new HubConnectionBuilder().WithUrl(connectionString, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            }).Build();
            connection.Closed += Connection_Closed;

            connection.On<Chat>("ReceiveChat", (chat) => ReceiveChat(chat));
            connection.On<Message>("ReceiveMessage", (message) => ReceiveMessage(message));
            connection.On<ChatUser>("UserLeft", (chatUser) => UserLeft(chatUser));

            connection.StartAsync();
        }
        public async Task LeaveChat(int chatId)
        {
           await connection.SendAsync("LeaveChat", chatId);
        }
        public void UserLeft(ChatUser chatUser)
        {
            DeleteChatCommand.Execute(new Chat { Id = chatUser.ChatId }).Subscribe();
        }
        public void ReceiveMessage(Message message)
        {
            LoadNewMessagesCommand.Execute(message).Subscribe();
        }
        public void ReceiveChat(Chat chat)
        {
            LoadNewChatCommand.Execute(chat).Subscribe();
        }
        private async Task Connection_Closed(Exception? arg)
        {
            ConnectionFailedCommand.Execute().Subscribe();

            await Task.CompletedTask;
        }
        public async Task CreateChat(Chat chat)
        {
            await connection.SendAsync("CreateChat", chat);
        }
        public async Task SendMessage(Message message)
        {
            await connection.SendAsync("SendMessage", message);
        }
    }
}
