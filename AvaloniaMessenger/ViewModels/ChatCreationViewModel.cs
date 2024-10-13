using Avalonia.Collections;
using Avalonia.Threading;
using AvaloniaMessenger.Controllers;
using AvaloniaMessenger.Models;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;


namespace AvaloniaMessenger.ViewModels
{
    internal class ChatCreationViewModel : ViewModelBase
    {
        private AvaloniaList<User> _addedUsersList;
        public AvaloniaList<User> AddedUsersList
        {
            get => _addedUsersList;
            set => this.RaiseAndSetIfChanged(ref _addedUsersList, value);
        }

        private AvaloniaList<User> _searchUsersList;
        public AvaloniaList<User> SearchUsersList
        {
            get => _searchUsersList;
            set => this.RaiseAndSetIfChanged(ref _searchUsersList, value);
        }

        private string _chatName;
        public string ChatName
        {
            get => _chatName;
            set => this.RaiseAndSetIfChanged(ref _chatName, value);
        }

        private string _searchField;
        public string SearchField
        {
            get => _searchField;
            set => this.RaiseAndSetIfChanged(ref _searchField, value);
        }

        public User? _chosenUserListItem;
        public User? ChosenUserListItem
        {
            get => _chosenUserListItem;
            set
            {
                try
                {
                    this.RaiseAndSetIfChanged(ref _chosenUserListItem, value);
                }
                catch(Exception e)
                {

                }
            }
        }
        public User? _removedUserListItem;
        public User? RemovedUserListItem
        {
            get => _removedUserListItem;
            set => this.RaiseAndSetIfChanged(ref _removedUserListItem, value);
        }

        public ReactiveCommand<Unit, Unit> CreateNewChatCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CloseViewCommand { get; set; }

        public SignalRController SignalRController;
        public MessengerController Messenger;
        public User CurrentUser;

        public ChatCreationViewModel()
        {
            SearchUsersList = new AvaloniaList<User>();
            AddedUsersList = new AvaloniaList<User>();

            var canCreateChat = this.WhenAnyValue(cn => cn.ChatName, cn => !String.IsNullOrEmpty(cn));
            CreateNewChatCommand = ReactiveCommand.Create(CreateNewChat, canCreateChat);

            this.WhenAnyValue(i => i.AddedUsersList, j => j.RemovedUserListItem).Subscribe(i => Task.Run(() => SearchUser(SearchField)));
            this.WhenAnyValue(i => i.SearchField).Subscribe(i => Task.Run(() => SearchUser(i)));
            this.WhenAnyValue(i => i.ChosenUserListItem).Subscribe(i => AddNewUser(i));
            this.WhenAnyValue(i => i.RemovedUserListItem).Subscribe(i => RemoveUser(i));
        }
        private void CreateNewChat()
        {
            var chatUsers = new List<ChatUser>
            {
                new ChatUser { UserId = CurrentUser.Id }
            };

            foreach(var i in AddedUsersList)
            {
                chatUsers.Add(new ChatUser()
                {
                    UserId = i.Id
                });
            }

            Chat newChat = new Chat
            {
                ChatName = ChatName,
                ChatUsers = chatUsers
            };

            var result = SignalRController.CreateChat(newChat);
        }
        private void RemoveUser(User user)
        {
            var userItem = AddedUsersList.FirstOrDefault(i => i == user);
            if (userItem == null)
                return;

            Dispatcher.UIThread.Post(() => AddedUsersList.Remove(user));
        }
        private void AddNewUser(User? user)
        {
            if (user == null)
                return;

            AddedUsersList.Add(user);

            Dispatcher.UIThread.Post(() =>
            {
                SearchUsersList.Remove(user);
            });
        }
        private async Task SearchUser(string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                SearchUsersList.Clear();
                return;
            }

            var users = new List<User>(Messenger.SearchUser(username));

            var newUsers = users.Where(u => 
            SearchUsersList.FirstOrDefault(i => u.Id == i.Id) == null 
            && AddedUsersList.FirstOrDefault(i => u.Id == i.Id) == null
            && u.Id != CurrentUser.Id);

            if (newUsers.Count() > 0)
                SearchUsersList = new AvaloniaList<User>(newUsers);
            else if (users.Count() == 0)
                SearchUsersList.Clear();


            await Task.CompletedTask;
        }

        private void CreateChat()
        {

        }
    }

}
