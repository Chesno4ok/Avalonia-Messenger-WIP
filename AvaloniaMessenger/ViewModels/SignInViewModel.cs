using AvaloniaMessenger.Models;
using Avalonia.Platform;
using Avalonia.Media;
using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using System.IO;
using Microsoft.Win32;
using AvaloniaMessenger.Assets;
using AvaloniaMessenger.Controllers;
using AvaloniaMessenger.Views;
using DynamicData;
using Avalonia.Threading;
using System.Security.Authentication;
using AvaloniaMessenger.Exceptions;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaMessenger.ViewModels
{
    class SignInViewModel : ViewModelBase
    {
        private string _login = String.Empty;
        public string Login 
        {
            get 
            { 
                return _login;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _login, value);
            }
        }
        // UserControls
        private PasswordInput _passwordInput = new PasswordInput();
        public PasswordInput PasswordInput { get => _passwordInput; set { this.RaiseAndSetIfChanged(ref _passwordInput, value); } }
        public PasswordInputViewModel _passwordInputViewModel = new PasswordInputViewModel();

        private ErrorListView _errorList = new();
        public ErrorListView ErrorList { get => _errorList; set { this.RaiseAndSetIfChanged(ref _errorList, value); } }
        private ErrorListViewModel _errorListViewModel = new ErrorListViewModel();

        // Commands
        public ReactiveCommand<Unit, Unit> SignInCommand { get; set; }
        public ReactiveCommand<User, Unit> SetMessengerCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SignUpCommand { get; set; }
        // Timers
        private DispatcherTimer _loginCheckTimer = new() 
        { 
            IsEnabled = true, 
            Interval = TimeSpan.FromMilliseconds(100)
        };
        // Others
        public MessengerController Messenger;
        public SignInViewModel()
        {
            // Commands
            var isValidObservable = this.WhenAnyValue(
                l => l.Login, p => p._passwordInputViewModel.Password, (l, p) => !String.IsNullOrEmpty(l) && !String.IsNullOrEmpty(p));

            SignInCommand = ReactiveCommand.CreateFromTask(SetMessenger, isValidObservable);
            SignInCommand.ThrownExceptions.Subscribe(i => HandleExceptions(i));

            // UserControls
            ErrorList.DataContext = _errorListViewModel;
            PasswordInput.DataContext = _passwordInputViewModel;
        }
        public void HandleExceptions(Exception exception)
        {
            if(exception is InvalidCredentialException)
            {
                _errorListViewModel.AddError("Invalid login or password");
            }
            else if(exception is NoConnectionException)
            {
                _errorListViewModel.AddError("Couldn't connect to server");
            }
        }
        public async Task SetMessenger()
        {
            User user = new User { Login = Login, Password = _passwordInputViewModel.Password };
            User? userToken = null;
            
            try
            {
                userToken = Messenger.SignIn(user.Login, user.Password);
            }
            catch
            {
                throw new NoConnectionException("Couldn't connect to server");
            }

            if (userToken != null)
                SetMessengerCommand.Execute(userToken).Subscribe();
            else
                throw new InvalidCredentialException("Invalid credentials");

        }
    }
}
