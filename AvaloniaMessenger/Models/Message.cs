using System;
using System.Collections.Generic;

namespace AvaloniaMessenger.Models;

public partial class Message
{
    public int Id { get; set; }

    public int User { get; set; }

    public int ChatId { get; set; }

    public DateTime Date { get; set; }

    public byte[]? Content { get; set; }

    public string Type { get; set; } = null!;

    public bool IsRead { get; set; }
}
