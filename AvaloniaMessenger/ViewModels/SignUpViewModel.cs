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
using AvaloniaMessenger.Assets;

namespace AvaloniaMessenger.ViewModels
{
    public class SignUpViewModel : ViewModelBase
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

        public ReactiveCommand<Unit, User> SignUpCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ReturnCommand { get; set; }
        public ReactiveCommand<Unit, Unit> TogglePasswordChar { get; private set; }
        public SignUpViewModel()
        {
            _eyeIcon = new Bitmap(AssetLoader.Open(new Uri(AssetManager.GetEyeIconPath(IsPasswordHidden))));

            this.WhenAnyValue(x => x.IsPasswordHidden).Subscribe(x => { ToggleEye(); });
            this.WhenAnyValue(x => x.Login).Subscribe(x => { CheckLogin(Login); });
            this.WhenAnyValue(x => x.Password).Subscribe(x => { CheckPassword(Password, RepeatPassword); });

            var isValidObservable = this.WhenAnyValue(
                l => l.Login, p => p.Password, rp => rp.RepeatPassword, u => u.Username,
                (l, p, rp, u) => !String.IsNullOrEmpty(l) && !String.IsNullOrEmpty(p) && !String.IsNullOrEmpty(rp) && !String.IsNullOrEmpty(u));
            

            SignUpCommand = ReactiveCommand.Create(() => new User() { 
                Login = this.Login, 
                Password = this.Password, 
                Name = this.Username }, isValidObservable);
            
            TogglePasswordChar = ReactiveCommand.Create(() => { IsPasswordHidden = !IsPasswordHidden; });
        }
        public void ToggleEye()
        {
            PasswordChar = IsPasswordHidden ? '•' : null;
            EyeIcon = new Bitmap(AssetLoader.Open(new Uri(AssetManager.GetEyeIconPath(IsPasswordHidden))));
        }

        public void CheckLogin(string login)
        {

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
