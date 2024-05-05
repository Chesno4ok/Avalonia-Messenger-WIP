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

namespace AvaloniaMessenger.ViewModels
{
    
    class SignUpViewModel : ViewModelBase
    {
        public enum Problems
        {
            TooLong,
            AlreadyExists
        };

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

        private string _password = String.Empty;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _password, value);
            }
        }

        private string _repeatPassword = String.Empty;
        public string RepeatPassword
        {
            get
            {
                return _repeatPassword;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _repeatPassword, value);
            }
        }

        private bool _isPasswordHidden = true;
        public bool IsPasswordHidden
        {
            get
            {
                return _isPasswordHidden;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _isPasswordHidden, value);
            }
        }

          private char? _passwordChar = '•';
        public char? PasswordChar
        {
            get
            {
                return _passwordChar;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _passwordChar, value);
            }
        }

        public IImage _eyeIcon;
        public IImage EyeIcon
        {
            get
            {
                return _eyeIcon;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _eyeIcon, value);
            }
        }

        public AvaloniaList<string> LoginErrors { get; private set; } = new AvaloniaList<string>();
        public AvaloniaList<string> PasswordErrors { get; private set; } = new AvaloniaList<string>();
        public AvaloniaList<string> RepeatPasswordErrors { get; private set; } = new AvaloniaList<string>();
        public AvaloniaList<string> SignInErrors { get; private set; } = new AvaloniaList<string>();

        public ReactiveCommand<Unit, Unit> SignUpCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ReturnCommand { get; set; }
        public ReactiveCommand<Unit, Unit> TogglePasswordChar { get; private set; }

        private MessengerController _messengerController { get; set; }
        public SignUpViewModel(MessengerController messengerController)
        {
            _messengerController = messengerController;
            _eyeIcon = new Bitmap(AssetLoader.Open(new Uri(AssetManager.GetEyeIconPath(IsPasswordHidden))));

            this.WhenAnyValue(x => x.IsPasswordHidden).Subscribe(x => { ToggleEye(); });
            this.WhenAnyValue(x => x.Login).Subscribe( x => { Task.Run(() => CheckLogin(Login)); });
            this.WhenAnyValue(x => x.Password).Subscribe(x => { CheckPassword(Password, RepeatPassword); });
            this.WhenAnyValue(x => x.RepeatPassword).Subscribe(x => { CheckPassword(Password, RepeatPassword); });

            var isValidObservable = this.WhenAnyValue(
                l => l.Login, p => p.Password, rp => rp.RepeatPassword, u => u.Username,
                (l, p, rp, u) => !String.IsNullOrEmpty(l) 
                && !String.IsNullOrEmpty(p) 
                && !String.IsNullOrEmpty(rp) 
                && !String.IsNullOrEmpty(u) 
                && PasswordErrors.Count == 0
                && LoginErrors.Count == 0
                && RepeatPasswordErrors.Count == 0);
            

            SignUpCommand = ReactiveCommand.Create(() => SignUp(), isValidObservable);
            
            TogglePasswordChar = ReactiveCommand.Create(() => { IsPasswordHidden = !IsPasswordHidden; });
        }
        public void ToggleEye()
        {
            PasswordChar = IsPasswordHidden ? '•' : null;
            EyeIcon = new Bitmap(AssetLoader.Open(new Uri(AssetManager.GetEyeIconPath(IsPasswordHidden))));
        }
        public void SignUp()
        {
            SignInErrors.Clear();

            var user = new User()
            {
                Login = this.Login,
                Password = this.Password,
                Name = this.Username
            };

            try
            {
                _messengerController.SignUp(user.Name, user.Login, user.Password);

                ReturnCommand.Execute().Subscribe();
            }
            catch
            {
                SignInErrors.Add("Failed to sign up");
            }
        }

        public async Task CheckLogin(string login)
        {
            if (login == "")
                return;

            if (_messengerController.CheckLogin(login) && !LoginErrors.Any(i => i == "Login already exists"))
                LoginErrors.Add("Login already exists");
            else
                LoginErrors.Remove("Login already exists");    

            if (Login.Length < 4 && !LoginErrors.Any(i => i == "Login is too short"))
                LoginErrors.Add("Login is too short");
            else if(Login.Length > 3 && LoginErrors.Any(i => i == "Login is too short"))
                LoginErrors.Remove("Login is too short");

        }
        public void CheckPassword(string password, string repeatPassword)
        {
            PasswordErrors.Clear();
            RepeatPasswordErrors.Clear();

            if (String.IsNullOrEmpty(password))
                return;

            if (password.Length < 8)
                PasswordErrors.Add("At least 8 symbols");
            if (!password.Any(i => Char.IsUpper(i)))
                PasswordErrors.Add("At least 1 uppercase letter");
            if (!password.Any(i => Char.IsNumber(i)))
                PasswordErrors.Add("At least 1 number");
            if (!password.Any(i => !Char.IsLetterOrDigit(i)))
                PasswordErrors.Add("At least 1 special symbol");
            if (password != repeatPassword)
                RepeatPasswordErrors.Add("Passwords must be the same");
        }
    }

    
}
