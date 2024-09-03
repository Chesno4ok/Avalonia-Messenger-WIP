using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaMessenger.Controllers;
using AvaloniaMessenger.Controls;
using AvaloniaMessenger.Models;
using AvaloniaMessenger.Views;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaMessenger.ViewModels
{
    class MessengerViewModel : ViewModelBase
    {
        private User user { get; set; }
        private MessengerController messenger { get; set; }

        // Binding Properties
        private Chat? _selectedChat;
        public Chat? SelectedChat
        {
            get => _selectedChat;
            set => this.RaiseAndSetIfChanged(ref _selectedChat, value);
        }
        private string? _messageText;
        public string? MessageText
        {
            get => _messageText;
            set => this.RaiseAndSetIfChanged(ref _messageText, value);
        }
        public List<SideBarButton> _sideBarButtons = new();
        public List<SideBarButton> SideBarButtons
        {
            get => _sideBarButtons;
            set => this.RaiseAndSetIfChanged(ref _sideBarButtons, value);
        }

        private AvaloniaList<Chat> _chatList = new AvaloniaList<Chat>();
        public AvaloniaList<Chat> ChatList
        {
            get => _chatList;
            set { this.RaiseAndSetIfChanged(ref _chatList, value); }
        }

        private List<User>? _userList = new List<User>();

        private AvaloniaList<Message> _messages = new AvaloniaList<Message>();
        public AvaloniaList<Message> Messages
        {
            get => _messages;
            set { this.RaiseAndSetIfChanged(ref _messages, value); }
        }

        private UserControl _popUpWindow;
        public UserControl PopUpWindow
        {
            get => _popUpWindow;
            set => this.RaiseAndSetIfChanged(ref _popUpWindow, value);
        }

        private ReactiveCommand<Unit, Unit> _toggleSideBarCommand;
        public ReactiveCommand<Unit, Unit> ToggleSideBarCommand { get => _toggleSideBarCommand; set => this.RaiseAndSetIfChanged(ref _toggleSideBarCommand, value); }

        public ReactiveCommand<Unit, Unit> SendMessageCommand { get; set; }
        public ReactiveCommand<Unit, Unit> LoadPreviousMessagesCommand { get; set; }
        public ReactiveCommand<Unit, Unit> LoadNewMessagesScrollCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ExitMessengerCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenChatCreationCommand { get; set; }
        // Additional properties
        public MessengerView MessengerView;
        public WebSocketController WebSocket;

        // Events


        public MessengerViewModel(User user, MessengerController messenger, ReactiveCommand<Unit, Unit> exitCommand)
        {
            this.user = user;
            this.messenger = messenger;
            SelectedChat = null;

            // WebSocket
            WebSocket = new WebSocketController($"wss://{messenger.apiCaller.ServerUrl.Host}:{messenger.apiCaller.ServerUrl.Port}" +
                $"/Message/ws_exchange_messages?userId={user.id}&token={user.Token}");
            WebSocket.LoadNewMessagesCommand = ReactiveCommand.Create<Message>(m => OnNewMessage(m));
            WebSocket.LoadNewChatCommand = ReactiveCommand.Create<Chat>(ch => OnNewChat(ch));
            WebSocket.ConnectionFailedCommand = ReactiveCommand.CreateFromTask<ClientWebSocket>(ws => OnConnectionFailed(ws));

            // Commands and Subscribes
            SendMessageCommand = ReactiveCommand.CreateFromTask(SendMessage, null);

            var canLoadPreviousMessages = this.WhenAnyValue(m => m.Messages, ch => ch.SelectedChat, (m, ch) => m.Count == 0);
            LoadPreviousMessagesCommand = ReactiveCommand.CreateFromTask(LoadPreviousMessages, canLoadPreviousMessages);

            this.WhenAnyValue(i => i.SelectedChat).Subscribe(i => Task.Run(() => ShowChat(i)));


            // Side bar

            OpenChatCreationCommand = ReactiveCommand.Create(OpenChatCreation);
            SideBarButtons.Add(new SideBarButton { CommandName = "Create new chat", ReactiveCommand = OpenChatCreationCommand });


            OpenSettingsCommand = ReactiveCommand.CreateFromTask(OpenSettings);
            SideBarButtons.Add(new SideBarButton { CommandName = "Settings", ReactiveCommand = OpenSettingsCommand });

            SideBarButtons.Add(new SideBarButton { CommandName = "Exit", ReactiveCommand = exitCommand });

            SetChats();
        }
        private void OpenChatCreation()
        {
            Dispatcher.UIThread.Post(() =>
            {
                ToggleSideBarCommand.Execute().Subscribe();

                var vm = new ChatCreationViewModel();
                vm.Messenger = messenger;
                vm.CurrentUser = user;
                vm.CloseViewCommand = ReactiveCommand.Create(ClosePopUpWindow);


                vm.CreateNewChatCommand.Subscribe(i => 
                {
                    PopUpWindow = null;
                });



                PopUpWindow = new ChatCreationView();
                PopUpWindow.DataContext = vm;
            });
        }

        private void ClosePopUpWindow()
        {
            PopUpWindow = null;
        }

        private async Task OpenSettings()
        {

        }
        private async Task LoadPreviousMessages()
        {
            int firstId = Messages[0].Id;

            var newMessages = messenger.GetPreviousMessages(user.id, user.Token, SelectedChat.Id, firstId, 10);

            newMessages = MarkSenders(newMessages);

            if (newMessages != null)
                Messages.InsertRange(0, newMessages.Reverse());
        }

        public async Task SendMessage()
        {
            var message = new Message
            {
                ChatId = SelectedChat.Id,
                User = user.id,
                Text = MessageText
            };

            MessageText = "";
            await WebSocket.SendMessage(message);
        }
        private void OnNewChat(Chat chat)
        {
            ChatList.Add(chat);
        }
        private void OnNewMessage(Message message)
        {
            message.Sender = messenger.GetUser(message.User).Name;

            LoadNewMessagesScrollCommand.Execute().Subscribe();
            
            Messages.Add(message);
        }
        private async Task OnConnectionFailed(ClientWebSocket webSocket)
        {
            var buffer = PopUpWindow;

            Dispatcher.UIThread.Post(() =>
            {
                PopUpWindow = null;
                PopUpWindow = new ReconnectingView();
            });

            while(WebSocket.WebSocket.State != WebSocketState.Open)
            {
                
            }

            Dispatcher.UIThread.Post(() => PopUpWindow = buffer);

        }

        private void ShowChat(Chat selectedChat)
        {
            if (selectedChat == null)
                return;

            Messages.Clear();
            Message[]? messages = messenger.GetLastMessages(user.id, user.Token, selectedChat.Id, 20);

            messages = MarkSenders(messages);

            Messages.AddRange(messages.Reverse());
            LoadNewMessagesScrollCommand.Execute().Subscribe();
        }
        private Message[] MarkSenders(IEnumerable<Message> messages)
        {
            _userList = new List<User>(messenger.GetChatUsers(user.id, user.Token, SelectedChat.Id));

            foreach (var m in messages)
            {
                m.Sender = _userList.FirstOrDefault(i => i.id == m.User).Name;
            }

            return messages.ToArray();
        }
        private void SetChats()
        {
            ChatList.AddRange(messenger.GetChats(user.id.ToString(), user.Token));
        }
    }
}
