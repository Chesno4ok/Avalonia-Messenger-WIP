using AvaloniaMessenger.Models;
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

namespace AvaloniaMessenger.ViewModels
{
    public class SignInViewModel : ViewModelBase
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

        

        public ReactiveCommand<Unit, UserInfo> SignInCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SignUpCommand { get; set; }
        public ReactiveCommand<Unit, Unit> TogglePasswordChar { get; private set; }
        public SignInViewModel()
        {
            var isValidObservable = this.WhenAnyValue(
                x => x.Login, x => x.Password, (l,p) => !String.IsNullOrEmpty(l) && !String.IsNullOrEmpty(p));
           
            _eyeIcon = new Bitmap($"{AssetManager.GetEyeIconPath(IsPasswordHidden)}");
            this.WhenAnyValue(x => x.IsPasswordHidden).Subscribe(x => { ToggleEye(); });


            SignInCommand = ReactiveCommand.Create(() => new UserInfo() { Login = this.Login, Password = this.Password }, isValidObservable);
            TogglePasswordChar = ReactiveCommand.Create(() => { IsPasswordHidden = !IsPasswordHidden; });
        }
        public void ToggleEye()
        {
            PasswordChar = IsPasswordHidden ? '•' : null;
            EyeIcon = new Bitmap($"{AssetManager.GetEyeIconPath(IsPasswordHidden)}");
        }
    }
}
