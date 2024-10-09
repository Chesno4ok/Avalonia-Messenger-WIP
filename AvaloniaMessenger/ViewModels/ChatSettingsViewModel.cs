using Avalonia.Collections;
using AvaloniaMessenger.Controllers;
using AvaloniaMessenger.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaMessenger.ViewModels
{
    class ChatSettingsViewModel : ViewModelBase
    {
        private Chat _currentChat;
        public Chat CurrentChat
        {
            get => _currentChat;
            set => this.RaiseAndSetIfChanged(ref _currentChat, value);
        }
        private AvaloniaList<User> _users = new();
        public AvaloniaList<User> Users
        {
            get => _users;
            set => this.RaiseAndSetIfChanged(ref _users, value);
        }
        public ReactiveCommand<Unit, Unit> LeaveChatCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CloseChatSettings { get; set; }
        private SignalRController signalRController;
        private MessengerController messenger;
        private User currentUser;

        public ChatSettingsViewModel(MessengerController messenger, SignalRController signalRController, User currentUser, Chat currentChat)
        {
            this.signalRController = signalRController;
            this.messenger = messenger;
            this.currentUser = currentUser;
            this.CurrentChat = currentChat;

            var chatUsers = messenger.GetChatUsers(currentUser.Id, CurrentChat.Id);
            Users.AddRange(chatUsers);

            LeaveChatCommand = ReactiveCommand.Create(() => LeaveChat());
        }
        private void LeaveChat()
        {
            signalRController.LeaveChat(CurrentChat.Id);
            CloseChatSettings.Execute().Subscribe();
        }


    }
}
