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
    class MessengerController
    {

        private ApiCaller apiCaller { get; set; }
        private QueryStringBuilder queryBuilder { get; set; }

        public MessengerController(Uri serverInfo)
        {
            apiCaller = new ApiCaller(serverInfo);

            queryBuilder = new QueryStringBuilder();
        }

        public User? SignIn(string login, string password)
        {
            User user = new User() { Login = login, Password = password};

            User? resUser = new User();

           // user = GetRequest<User>("/User/get_token", query, null);

            return user;
        }
        public User SignUp(string login, string password, string name)
        {
            User user = new User() { Login = login, Password = password, Name = name };

            User? resUser = new User();

           // resUser = GetRequest<User>("/User/register_user?", query, new StringContent(""));

            return user;
        }
        [QueryInfo("login")]
        public bool CheckLogin(string login)
        {
            User user = new User() { Login = login };
            var query = queryBuilder.GetQueryString(login);

            try
            {
                apiCaller.GetRequest("User/check_login?", query);
                return false;
            }
            catch(HttpRequestException e)
            {
                return true;
            }
            
        }
        
    }
    

}