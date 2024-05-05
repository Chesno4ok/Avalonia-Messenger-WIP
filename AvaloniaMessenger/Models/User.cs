using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AvaloniaMessenger.Models;

public class User
{

    public int id { get; set; }

    public string name { get; set; } = "";

    public string Login { get; set; } = "";

    public string Password { get; set; } = "";

    public string Token { get; set; } = "";
}
