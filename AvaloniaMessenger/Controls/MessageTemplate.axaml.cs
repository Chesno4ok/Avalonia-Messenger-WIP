using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using ReactiveUI;
using System;
using System.Text;

namespace AvaloniaMessenger.Controls
{
    public class MessageTemplate : TemplatedControl
    {
        public MessageTemplate()
        {
            ContentProperty.WhenAnyValue(i => i).Subscribe(i =>  SetContent());
        }

        public void SetContent()
        {

        }

        public static readonly StyledProperty<string> SenderProperty =
            AvaloniaProperty.Register<MessageTemplate, string>(nameof(Sender));
        public string Sender
        {
            get => GetValue(SenderProperty);
            set => SetValue(SenderProperty, value);
        }

        public static readonly StyledProperty<string> TextMessageProperty =
           AvaloniaProperty.Register<MessageTemplate, string>(nameof(TextMessage));
        public string TextMessage
        {
            get => GetValue(TextMessageProperty);
            set => SetValue(TextMessageProperty, value);
        }

        public readonly static StyledProperty<byte[]> ContentProperty  =
           AvaloniaProperty.Register<MessageTemplate, byte[]>(nameof(Content));
        public byte[] Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);

        }

        public static readonly StyledProperty<string> TimeProperty =
          AvaloniaProperty.Register<MessageTemplate, string>(nameof(Time));
        public string Time
        {
            get => GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }
    }
}
