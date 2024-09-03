using System;
using System.Collections.Generic;
using System.Reactive;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaMessenger.Assets;
using ReactiveUI;
using Splat;

namespace AvaloniaMessenger.ViewModels
{
	public class PasswordInputViewModel : ViewModelBase
	{
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
        private string _watermark = "Password";
        public string Watermark
        {
            get
            {
                return _watermark;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _watermark, value);
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
        public ReactiveCommand<Unit, Unit> TogglePasswordChar { get; private set; }
        public PasswordInputViewModel()
        {
            EyeIcon = new Bitmap(AssetLoader.Open(new Uri(AssetManager.GetEyeIconPath(IsPasswordHidden))));
            TogglePasswordChar = ReactiveCommand.Create(ToggleEye);
        }
        public void ToggleEye()
        {
            IsPasswordHidden = !IsPasswordHidden;
            PasswordChar = IsPasswordHidden ? '•' : null;
            EyeIcon = new Bitmap(AssetLoader.Open(new Uri(AssetManager.GetEyeIconPath(IsPasswordHidden))));
        }
    }
}