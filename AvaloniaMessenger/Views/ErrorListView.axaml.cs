using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using System;

namespace AvaloniaMessenger.Views
{
    public partial class ErrorListView : UserControl
    {
        public ErrorListView()
        {
            InitializeComponent();
        }
        public void ShowAnimation(Control control)
        {
            Animation showingAnimation = new();
            showingAnimation.Duration = TimeSpan.FromSeconds(3);
            showingAnimation.IterationCount = new IterationCount(1);
            showingAnimation.Easing = Easing.Parse("QuinticEaseOut");

            KeyFrame keyFrame1 = new KeyFrame();
            keyFrame1.KeyTime = TimeSpan.FromSeconds(0);
            keyFrame1.Setters.Add(new Avalonia.Styling.Setter(Canvas.LeftProperty, 300));
            keyFrame1.Setters.Add(new Avalonia.Styling.Setter(OpacityProperty, 0));
            keyFrame1.Setters.Add(new Avalonia.Styling.Setter(HeightProperty, 300));
        }
    }
}
