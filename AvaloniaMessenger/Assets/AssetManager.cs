using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace AvaloniaMessenger.Assets
{
    static class AssetManager
    {
        private static bool _isLightTheme = false;

        private static string _baseUri = "avares://AvaloniaMessenger/Assets/";
        private static string _openEyeDark = _baseUri + "eye_open_dark.png";
        private static string _closeEyeDark = _baseUri + "eye_close_dark.png";

        private static string _openEyeLight = _baseUri + "eye_open_light.png";
        private static string _closeEyeLight = _baseUri + "eye_close_light.png";

        public static string GetEyeIconPath(bool IsPasswordHidden)
        {
            if (IsPasswordHidden)
                return _isLightTheme ? _openEyeLight : _openEyeDark;

            return _isLightTheme ? _closeEyeLight : _closeEyeDark;
        }

        public static Bitmap LoginIcon { get => new Bitmap(AssetLoader.Open(new Uri(_isLightTheme ? _loginLight : _loginDark))); }
        private static string _loginLight = _baseUri + "login_light.png";
        private static string _loginDark = _baseUri + "login_dark.png";

        public static Bitmap PasswordIcon { get => new Bitmap(AssetLoader.Open(new Uri(_isLightTheme ? _passwordLight : _passwordDark))); }
        private static string _passwordLight = _baseUri + "password_light.png";
        private static string _passwordDark = _baseUri + "password_dark.png";

        public static Bitmap UsernameIcon { get => new Bitmap(AssetLoader.Open(new Uri(_isLightTheme ? _usernameLight : _usernameDark))); }
        private static string _usernameLight = _baseUri + "username_light.png";
        private static string _usernameDark = _baseUri + "username_dark.png";

    }
}
