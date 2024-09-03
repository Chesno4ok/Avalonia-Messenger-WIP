using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaMessenger.Controls
{
    public class ChatButton : TemplatedControl
    {
        public static readonly StyledProperty<string> ChatNameProperty = 
            AvaloniaProperty.Register<ChatButton, string>(nameof(ChatName));

        public string ChatName
        {
            get => GetValue(ChatNameProperty);
            set => SetValue(ChatNameProperty, value);
        }

        public static readonly StyledProperty<string> LastMessageProperty = 
            AvaloniaProperty.Register<ChatButton, string>(nameof(LastMessage));

        public string LastMessage
        {
            get => GetValue(LastMessageProperty);
            set => SetValue(LastMessageProperty, value);
        }
    }
}
