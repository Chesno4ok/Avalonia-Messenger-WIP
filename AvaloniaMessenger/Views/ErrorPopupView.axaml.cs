using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Rendering.Composition.Animations;
using System.Threading;

namespace AvaloniaMessenger.Views
{
    public partial class ErrorPopupView : UserControl
    {
        public ErrorPopupView()
        {
            InitializeComponent();
        }
        public void HideAnimation()
        {
            ErrorPopup.Classes.Add("delete");
        }
    }
}
