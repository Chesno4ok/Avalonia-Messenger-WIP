using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaMessenger
{
    static class AssetManager
    {
        private static bool _isLightTheme = false;

        private static string _openEyeDark = "Assets/eye_open_dark.png";
        private static string _closeEyeDark = "Assets/eye_close_dark.png";

        private static string _openEyeLight = "Assets/eye_open_light.png";
        private static string _closeEyeLight = "Assets/eye_close_light.png";

        public static string GetEyeIconPath(bool IsPasswordHidden)
        {
            if (IsPasswordHidden)
                return _isLightTheme ? _openEyeLight : _openEyeDark;

            return _isLightTheme ? _closeEyeLight : _closeEyeDark;
        }
    }
}
