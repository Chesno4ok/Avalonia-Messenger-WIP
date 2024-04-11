using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AvaloniaMessenger.Models;

[JsonObject(MemberSerialization.OptIn)]
public partial class ChatUser
{
    [JsonProperty]
    public int Id { get; set; }
    [JsonProperty]
    public int UserId { get; set; }
    [JsonProperty]
    public int ChatId { get; set; }
    [JsonProperty]
    public bool HasUpdates { get; set; }
}
