using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Data;
using AvaloniaMessenger.Controllers;
using AvaloniaMessenger.Models;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace AvaloniaMessenger.ViewModels
{
    internal class UserProfileViewModel : ViewModelBase
    {
        private User _user;

        private string _userName;

        [Required]
        public string UserName
        {
            get => _userName;
            set
            {
                if (value.Length < 4)
                    throw new DataValidationException("Length must be at least 4 symbols");

                _user.Name = value;
                this.RaiseAndSetIfChanged(ref _userName, value);
            }
        }

        private MessengerController _messenger;
        public ReactiveCommand<User, Unit>? ExitUserProfile { get; set; }

        public UserProfileViewModel(User user, MessengerController messenger)
        {
            _user = user;
            UserName = user.Name;
            _messenger = messenger;
        }

        public void SaveUser()
        {
            if (UserName.Length < 4)
                return;


            var newUser = _messenger.EditUser(_user);

            if (newUser is null)
                return;

            ExitUserProfile.Execute(newUser).Subscribe();
        }
    }
}
