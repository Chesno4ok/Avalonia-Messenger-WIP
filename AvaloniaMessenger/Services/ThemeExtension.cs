using Avalonia;
using Avalonia.Themes.Neumorphism;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaMessenger.Services
{
    public static class ThemeExtension
    {
        public static T LocateMaterialTheme<T>(this Application application) where T : NeumorphismTheme
        {
            var materialTheme = application.Styles.FirstOrDefault(style => style is T);
            if (materialTheme == null)
            {
                throw new InvalidOperationException(
                    $"Cannot locate {nameof(T)} in Avalonia application styles. Be sure that you include MaterialTheme in your App.xaml in Application.Styles section");
            }

            return (T)materialTheme;
        }
    }
}
