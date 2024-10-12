using Avalonia;
using Avalonia.Controls.Primitives;
using System;

namespace AvaloniaMessenger.Controls;

public class MessageDateTemplate : MessageTemplate
{
    public readonly static StyledProperty<DateTime> DateProperty =
           AvaloniaProperty.Register<MessageDateTemplate, DateTime>(nameof(Date));
    public DateTime Date
    {
        get => GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }
}