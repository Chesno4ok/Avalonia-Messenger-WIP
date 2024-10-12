using Avalonia.Media;
using AvaloniaMessenger.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media.Imaging;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Platform;
using AvaloniaMessenger.Controllers;
using AvaloniaMessenger.Assets;
using AvaloniaMessenger.Views;
using AvaloniaMessenger.ViewModels;
using Avalonia.Threading;

namespace AvaloniaMessenger.ViewModels
{
    
    class SignUpViewModel : ViewModelBase
    {
        private string _username = String.Empty;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _username, value);
            }
        }

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

        // Password inputs
        private PasswordInput _password = new();
        public PasswordInputViewModel _passwordViewModel = new();
        public PasswordInput Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        private PasswordInput _repeatPassword = new ();
        public PasswordInputViewModel _repeatPasswordViewModel = new() { Watermark = "Password Again"};
        public PasswordInput RepeatPassword
        {
            get => _repeatPassword;
            set => this.RaiseAndSetIfChanged(ref _repeatPassword, value);
        }

        // Errors
        private ErrorListViewModel _loginErrorsViewModel = new();
        private ErrorListView _loginErrors = new();
        public ErrorListView LoginErrors { get => _loginErrors; set => this.RaiseAndSetIfChanged(ref _loginErrors, value); }

        private ErrorListViewModel _passwordErrorsViewModel = new();
        private ErrorListView _passwordErrors = new();
        public ErrorListView PasswordErrors { get => _passwordErrors; set => this.RaiseAndSetIfChanged(ref _passwordErrors, value); }

        private ErrorListViewModel _repeatErrorsViewModel = new();
        private ErrorListView _repeatPasswordErrors = new();
        public ErrorListView RepeatPasswordErrors { get => _repeatPasswordErrors; set => this.RaiseAndSetIfChanged(ref _repeatPasswordErrors, value); }

        private ErrorListViewModel _signInErrorsViewModel = new();
        private ErrorListView _signInErrors = new();
        public ErrorListView SignInErrors { get => _signInErrors; set => this.RaiseAndSetIfChanged(ref _signInErrors, value); }

        // Commands
        public ReactiveCommand<Unit, Unit> SignUpCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ReturnCommand { get; set; }
        // Timers
        private DispatcherTimer _checkLoginTimer = new() {Interval = TimeSpan.FromMilliseconds(500) };
        private DispatcherTimer _checkPasswordTimer = new() { Interval = TimeSpan.FromMilliseconds(500) };
        private DispatcherTimer _checkRepeatPasswordTimer = new() { Interval = TimeSpan.FromMilliseconds(500) };
        // Messenger Controller
        private MessengerController _messengerController { get; set; }
        public SignUpViewModel(MessengerController messengerController)
        {
            // Password Inputs
            Password.DataContext = _passwordViewModel;
            RepeatPassword.DataContext = _repeatPasswordViewModel;

            // Error lists
            LoginErrors.DataContext = _loginErrorsViewModel;
            PasswordErrors.DataContext = _passwordErrorsViewModel;
            RepeatPasswordErrors.DataContext = _repeatErrorsViewModel;
            SignInErrors.DataContext = _signInErrorsViewModel;

            // Messenger Controller
            _messengerController = messengerController;

            // Commands
            var isValidObservable = this.WhenAnyValue(
                l => l.Login, p => p._passwordViewModel.Password, rp => rp._repeatPasswordViewModel.Password, u => u.Username,

                (l, p, rp, u) => !String.IsNullOrEmpty(l) 
                && !String.IsNullOrEmpty(p) 
                && !String.IsNullOrEmpty(rp) 
                && !String.IsNullOrEmpty(u) 
                && _passwordErrorsViewModel.ErrorList.Count() == 0
                && _loginErrorsViewModel.ErrorList.Count() == 0
                && _repeatErrorsViewModel.ErrorList.Count == 0);
            SignUpCommand = ReactiveCommand.Create(() => SignUp(), isValidObservable);

            this.WhenAnyValue(i => i.Login).Subscribe(i => _checkLoginTimer.IsEnabled = true);
            this.WhenAnyValue(i => i._passwordViewModel.Password).Subscribe(i => TogglePasswordCheck());
            this.WhenAnyValue(i => i._repeatPasswordViewModel.Password).Subscribe(i => ToggleRepeatPasswordCheck());

            // Timers
            _checkLoginTimer.Tick += CheckLoginTick;
            _checkPasswordTimer.Tick += CheckPasswordTick;
            _checkRepeatPasswordTimer.Tick += CheckRepeatPasswordTick;
        }
        private void TogglePasswordCheck()
        {
            if (_passwordViewModel.Password != "")
                _checkPasswordTimer.IsEnabled = true;
        }
        private void ToggleRepeatPasswordCheck()
        {
            if (_repeatPasswordViewModel.Password != "")
                _checkRepeatPasswordTimer.IsEnabled = true;
        }
        private void CheckRepeatPasswordTick(object? sender, EventArgs e)
        {
            string password = _passwordViewModel.Password;
            string repeatPassword = _repeatPasswordViewModel.Password;

            if (password != repeatPassword)
                _repeatErrorsViewModel.AddError("Passwords not the same");
            else
                _repeatErrorsViewModel.RemoveError("Passwords not the same");
        }

        private void CheckPasswordTick(object? sender, EventArgs e)
        {
            string password = _passwordViewModel.Password;

            if (password.Length < 8)
                _passwordErrorsViewModel.AddError("At least 8 symbols");
            else
                _passwordErrorsViewModel.RemoveError("At least 8 symbols");

            if (!password.Any(i => Char.IsUpper(i)))
                _passwordErrorsViewModel.AddError("At least 1 uppercase letter");
            else
                _passwordErrorsViewModel.RemoveError("At least 1 uppercase letter");

            if (!password.Any(i => Char.IsNumber(i)))
                _passwordErrorsViewModel.AddError("At least 1 number");
            else
                _passwordErrorsViewModel.RemoveError("At least 1 number");

            if (!password.Any(i => !Char.IsLetterOrDigit(i)))
                _passwordErrorsViewModel.AddError("At least 1 special symbol");
            else
                _passwordErrorsViewModel.RemoveError("At least 1 special symbol");
        }

        private void CheckLoginTick(object? sender, EventArgs e)
        {
            Task.Run(() => CheckLogin(Login));
            _checkLoginTimer.IsEnabled = false;
            //CheckLogin(Login);
        }

        public void SignUp()
        {
            var user = new User()
            {
                Login = this.Login,
                Password = this._passwordViewModel.Password,
                Name = this.Username
            };

            try
            {
                _messengerController.SignUp(user.Name, user.Login, user.Password);

                ReturnCommand.Execute().Subscribe();
            }
            catch
            {
                _signInErrorsViewModel.AddError("Failed to sign up");
            }
        }

        public async Task CheckLogin(string login)
        {
            if (login == "")
                return;

            if (_messengerController.CheckLogin(login))
                _loginErrorsViewModel.AddError("Login already exists");
            else
                _loginErrorsViewModel.RemoveError("Login already exists");    

            if (Login.Length < 5 )
                _loginErrorsViewModel.AddError("Login is too short");
            else
                _loginErrorsViewModel.RemoveError("Login is too short");

            await Task.CompletedTask;
        }
    }

    
}
