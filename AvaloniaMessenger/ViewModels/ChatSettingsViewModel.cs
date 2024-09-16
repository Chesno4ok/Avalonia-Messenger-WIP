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
        private MessengerController messenger;
        private User currentUser;

        public ChatSettingsViewModel(MessengerController messenger, User currentUser, Chat currentChat)
        {
            this.messenger = messenger;
            this.currentUser = currentUser;
            this.CurrentChat = currentChat;

            var chatUsers = messenger.GetChatUsers(currentUser.id, currentUser.Token, CurrentChat.Id);
            Users.AddRange(chatUsers);

            LeaveChatCommand = ReactiveCommand.Create(() => LeaveChat());
        }
        private void LeaveChat()
        {
            messenger.LeaveChat(currentUser.id, currentUser.Token, CurrentChat.Id);
            CloseChatSettings.Execute().Subscribe();
        }


    }
}
