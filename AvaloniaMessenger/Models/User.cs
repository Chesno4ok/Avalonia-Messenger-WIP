using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AvaloniaMessenger.Models;

[JsonObject(MemberSerialization.OptIn)]
public partial class User
{
    [JsonProperty]
    public int Id { get; set; }
    [JsonProperty]
    public string Name { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Token { get; set; } = null!;
}
