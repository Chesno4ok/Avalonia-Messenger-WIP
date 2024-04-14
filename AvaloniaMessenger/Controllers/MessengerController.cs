using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using AvaloniaMessenger.Models;
using Newtonsoft.Json;

namespace AvaloniaMessenger.Controllers
{
    static class MessengerController
    {
        private static string _serverUrl { get; set; } = "https://localhost:7284";


        public static User? SignIn(string login, string password)
        {
            User user = new User() { Login = login, Password = password};

            User? resUser = new User();

           // user = GetRequest<User>("/User/get_token", query, null);

            return user;
        }
        public static User SignUp(string login, string password, string name)
        {
            User user = new User() { Login = login, Password = password, Name = name };

            User? resUser = new User();

           // resUser = GetRequest<User>("/User/register_user?", query, new StringContent(""));

            return user;
        }
        //public static User[]? SearchUser(string username)
        //{

        //    User[]? users = GetRequest<User[]>("/User/search_user?", query, null);

        //    return users;
        //}
        [QueryInfo("login")]
        public static bool CheckLogin(string login)
        {
            User user = new User() { Login = login };
            var query = GetQueryDictionary(login);

            try
            {
                GetRequest<User[]>("/User/check_login?", query);
                return true;
            }
            catch(HttpRequestException e)
            {
                return false;
            }
            
        }
        
    }
    

}