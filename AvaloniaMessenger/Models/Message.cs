using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AvaloniaMessenger.Models;

[JsonObject(MemberSerialization.OptIn)]
public partial class Message
{
    [JsonProperty]
    public int Id { get; set; }
    [JsonProperty]
    public int User { get; set; }
    [JsonProperty]
    public int ChatId { get; set; }
    [JsonProperty]
    public DateTime Date { get; set; }
    [JsonProperty]
    public byte[]? Content { get; set; }
    [JsonProperty]
    public string Type { get; set; } = null!;
    [JsonProperty]
    public bool IsRead { get; set; }

}
