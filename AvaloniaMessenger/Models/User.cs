using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AvaloniaMessenger.Models;
public class User
{

    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Login { get; set; } = "";

    public string Password { get; set; } = "";

    public string Token { get; set; } = "";
}
public class UserResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}
