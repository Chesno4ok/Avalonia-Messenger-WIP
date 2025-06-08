using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using AvaloniaMessenger.Models;
using ReactiveUI;
using System;
using System.Text;

namespace AvaloniaMessenger.Controls
{
    public class MessageTemplate : TemplatedControl
    {
        public MessageTemplate()
        {
            
        }

        public readonly static StyledProperty<Message> MessageProperty =
           AvaloniaProperty.Register<MessageTemplate, Message>(nameof(Message));
        public Message Message
        {
            get => GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public readonly static StyledProperty<bool> IsVisibleProperty =
           AvaloniaProperty.Register<MessageTemplate, bool>(nameof(IsVisible));
        public bool IsVisible
        {
            get => GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        public readonly static StyledProperty<string> DateProperty =
           AvaloniaProperty.Register<MessageTemplate, string>(nameof(Date));
        public string Date
        {
            get => GetValue(DateProperty);
            set
            {
                if (Message.User != null)
                    return;

                SetValue(DateProperty, value);
            }
        }

        public override void BeginInit()
        {
            this.WhenAnyValue(i => i.Message).Subscribe(i =>
            {
                if (i == null)
                    return;

                IsVisible = i.User != null;
                Date = Message.Date.ToString("D");
            });

            base.BeginInit();
        }
    }
}
