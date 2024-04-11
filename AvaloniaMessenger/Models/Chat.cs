using System;
using System.Collections.Generic;

namespace AvaloniaMessenger.Models;

public partial class Chat
{
    public int Id { get; set; }

    public string ChatName { get; set; } = null!;
}
