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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive;
using System.Reactive.Linq;
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

        private System.Threading.ReaderWriterLock locker = new ();
        private AvaloniaList<Message> _messages  = new AvaloniaList<Message>();
        public AvaloniaList<Message> Messages
        {
            get
            {
                lock (locker)
                {
                    return _messages;
                }
               
            }
            set 
            {
                lock (locker)
                {
                    this.RaiseAndSetIfChanged(ref _messages, value);
                }
                    
            }
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
        public ReactiveCommand<Unit, Unit> OpenChatSettingsCommand { get; set; }
        // Additional properties
        public MessengerView MessengerView;
        public SignalRController SignalRController;
        public bool EndOfChat = false;

        // Events


        public MessengerViewModel(User user, MessengerController messenger, ReactiveCommand<Unit, Unit> exitCommand)
        {
            this.user = user;
            this.messenger = messenger;
            SelectedChat = null;

            // SignalR
            SignalRController = new("https://localhost:7284/chat", user.Token, user);
            SignalRController.LoadNewMessagesCommand = ReactiveCommand.Create<Message>(m => OnNewMessage(m));
            SignalRController.LoadNewChatCommand = ReactiveCommand.Create<Chat>(ch => OnNewChat(ch));
            SignalRController.ConnectionFailedCommand = ReactiveCommand.CreateFromTask(OnConnectionFailed);
            SignalRController.DeleteChatCommand = ReactiveCommand.Create<Chat>(ch => DeleteChat(ch));

            // Commands and Subscribes
            SendMessageCommand = ReactiveCommand.CreateFromTask(SendMessage, null);
            OpenChatSettingsCommand = ReactiveCommand.Create(OpenChatSettings);
            
            var canLoadPreviousMessages = this.WhenAnyValue(m => m.Messages, ch => ch.SelectedChat, (m, ch) => m.Count == 0);
            LoadPreviousMessagesCommand = ReactiveCommand.CreateFromTask(LoadPreviousMessages, canLoadPreviousMessages);

            this.WhenAnyValue(i => i.SelectedChat).Subscribe(i => ShowChat(i));


            // Side bar

            OpenChatCreationCommand = ReactiveCommand.Create(OpenChatCreation);
            SideBarButtons.Add(new SideBarButton { CommandName = "Create new chat", ReactiveCommand = OpenChatCreationCommand });

            OpenSettingsCommand = ReactiveCommand.CreateFromTask(OpenSettings);
            SideBarButtons.Add(new SideBarButton { CommandName = "Settings", ReactiveCommand = OpenSettingsCommand });

            SideBarButtons.Add(new SideBarButton { CommandName = "Exit", ReactiveCommand = exitCommand });

            SetChats();
        }
        private void OpenChatSettings()
        {
            var vm = new ChatSettingsViewModel(messenger,SignalRController, user, SelectedChat)
            {
                CloseChatSettings = ReactiveCommand.Create(() => ClosePopUpWindow())
            };

            var view = new ChatSettingsView()
            {
                DataContext = vm
            };

            PopUpWindow = view;

        }
        private void DeleteChat(Chat chat)
        {
            if(SelectedChat.Id == chat.Id)
            {
                SelectedChat = null;
                Messages.Clear();
            }

            ChatList.Remove(ChatList.FirstOrDefault(i => i.Id == chat.Id));
        }
        private void OpenChatCreation()
        {
            Dispatcher.UIThread.Post(() =>
            {
                ToggleSideBarCommand.Execute().Subscribe();

                var vm = new ChatCreationViewModel();
                vm.Messenger = messenger;
                vm.SignalRController = SignalRController;
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
            int firstId = Messages.FirstOrDefault(i => i.Id != 0).Id;

            var newMessages = messenger.GetPreviousMessages(user.Id, SelectedChat.Id, firstId, 10);

            if(newMessages.Length == 0)
            {
                EndOfChat = true;
            }

            //newMessages = MarkSenders(newMessages);

            if (newMessages != null)
            {
                Messages.InsertRange(0, newMessages.Reverse());
                SetDates();
            }
            
            await Task.CompletedTask;
        }

        public async Task SendMessage()
        {
            var message = new Message
            {
                ChatId = SelectedChat.Id,
                UserId = user.Id,
                Text = MessageText
            };

            MessageText = "";
            await SignalRController.SendMessage(message);
        }
        private void OnNewChat(Chat chat)
        {
            ChatList.Add(chat);
        }
        private void OnNewMessage(Message message)
        {
            //message.Sender = _userList.FirstOrDefault(i => i.Id == message.UserId).Name;


            if (SelectedChat == null)
                return;


            var lastMessage = Messages.Count() == 0 ? new Message() : Messages.Last();

            

            Message oldMessage;
            while (true)
            {
                try
                {
                    oldMessage = Messages.FirstOrDefault(i => i.Id == message.Id);
                    break;
                }
                catch
                {

                }
            }

            if (lastMessage.Date.Date != message.Date.Date)
            {
                Messages.Add(new Message { Date = message.Date, User = null });
            }

            if (oldMessage != null)
                oldMessage = message;
            else if(message.ChatId == SelectedChat.Id)
                Messages.Add(message);

            

            LoadNewMessagesScrollCommand.Execute().Subscribe();
        }
        private async Task OnConnectionFailed()
        {
            if(!(PopUpWindow is ReconnectingView))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    PopUpWindow = new ReconnectingView();
                });
            }
            else
            {
                PopUpWindow = null;
            }

            
        }

        private async Task ShowChatAsync(Chat selectedChat)
        {
            if (selectedChat == null)
                return;

            var chat = messenger.GetChat(selectedChat.Id);
            if (chat.ChatUsers.All(i => i.UserId != user.Id))
            {
                SelectedChat = null;
                ChatList.Remove(ChatList.FirstOrDefault(i => i.Id == chat.Id));
                Messages.Clear();
                return;
            }

            LoadNewMessagesScrollCommand.Execute().Subscribe();
            EndOfChat = false;
            Messages.Clear();
            Message[]? messages = messenger.GetLastMessages(user.Id, selectedChat.Id, 20);

            //messages = MarkSenders(messages);

            Messages.AddRange(messages.Reverse());

            SetDates();
        }
        public void SetDates()
        {
            Message? prev = null;

            var dates = Messages.Where(i => i.User == null);
            
            if(dates.Count() > 0)
                Messages.RemoveAll(dates);

            foreach (var i in Messages)
            {
                if (prev == null)
                {
                    prev = i;
                    continue;
                }

                if (prev.Date.Date == i.Date.Date)
                {
                    prev = i;
                    continue;
                }

                Dispatcher.UIThread.Post(() => Messages.Insert(Messages.IndexOf(i), new Message { Date = i.Date, User = null }));
                prev = i;

            }
        }

        private void ShowChat(Chat selectedChat)
        {
            Task.Run(() => ShowChatAsync(selectedChat));
        }
        //private Message[] MarkSenders(IEnumerable<Message> messages)
        //{
        //    _userList = new List<User>(messenger.GetChatUsers(user.Id, SelectedChat.Id));

        //    foreach (var m in messages)
        //    {
        //        m.Sender = _userList.FirstOrDefault(i => i.Id == m.UserId).Name;
        //    }

        //    return messages.ToArray();
        //}
        private void SetChats()
        {
            ChatList.AddRange(messenger.GetChats(user.Id.ToString()));
        }
    }
}
