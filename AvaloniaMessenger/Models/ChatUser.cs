using System;
using System.Collections.Generic;

namespace AvaloniaMessenger.Models;

public partial class ChatUser
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ChatId { get; set; }

    public bool HasUpdates { get; set; }
}
