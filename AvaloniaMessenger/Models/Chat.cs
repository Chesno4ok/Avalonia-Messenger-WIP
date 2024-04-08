using System;
using System.Collections.Generic;

namespace ChesnokMessengerAPI;

public partial class Chat
{
    public int Id { get; set; }

    public string ChatName { get; set; } = null!;

    public virtual ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();
}
