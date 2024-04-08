using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

public partial class ChatUser
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ChatId { get; set; }

    public bool HasUpdates { get; set; }

    public virtual Chat Chat { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
