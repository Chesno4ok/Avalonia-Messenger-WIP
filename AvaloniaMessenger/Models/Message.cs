using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaMessenger.Models;

public partial class Message
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ChatId { get; set; }

    public DateTimeOffset Date { get; set; }

    public short IsRead { get; set; }

    public string? Text { get; set; }
    public string Time { get => Date.ToLocalTime().ToString("H:mm"); }
    public string Sender 
    {
        get => User == null ? " " : User.Name;
    }
    public UserResponse? User { get; set; } = null!;

}
