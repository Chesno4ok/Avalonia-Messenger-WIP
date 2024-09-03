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

        public MessengerController Messenger;
        public User CurrentUser;

        public ChatCreationViewModel()
        {
            SearchUsersList = new AvaloniaList<User>();
            AddedUsersList = new AvaloniaList<User>();

            var canCreateChat = this.WhenAnyValue(cn => cn.ChatName, cn => !String.IsNullOrEmpty(cn));
            CreateNewChatCommand = ReactiveCommand.Create(CreateNewChat, canCreateChat);

            this.WhenAnyValue(i => i.AddedUsersList, j => j.RemovedUserListItem).Subscribe(i => SearchUser(SearchField));
            this.WhenAnyValue(i => i.SearchField).Subscribe(i => SearchUser(i));
            this.WhenAnyValue(i => i.ChosenUserListItem).Subscribe(i => AddNewUser(i));
            this.WhenAnyValue(i => i.RemovedUserListItem).Subscribe(i => RemoveUser(i));
        }
        private void CreateNewChat()
        {
            var jsonContent = JsonConvert.SerializeObject(AddedUsersList.Select(i => i.id));
            var httpContent = new StringContent(jsonContent);
            var result = Messenger.CreateChat(CurrentUser.id, CurrentUser.Token, ChatName, httpContent);
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
        private void SearchUser(string username)
        {
            SearchUsersList.Clear();

            if (String.IsNullOrEmpty(username))
                return;

            var users = new List<User>(Messenger.SearchUser(username));

            users.RemoveMany(users.Where(i => AddedUsersList.Any(u => u.id == i.id) || i.id == CurrentUser.id));
            
            SearchUsersList.AddRange(users);
        }

        private void CreateChat()
        {

        }
    }

}
