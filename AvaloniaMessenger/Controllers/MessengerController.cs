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

        [QueryInfo("login", "password")]
        public User? SignIn(string login, string password)
        {
            var query = queryBuilder.GetQueryString(login, password);

            User? user = apiCaller.GetRequest<User>("/User/get_token", query);

            return user;
        }
        [QueryInfo("name", "login", "password")]
        public void SignUp(string name, string login, string password)
        {
            var query = queryBuilder.GetQueryString(name, login, password);

            apiCaller.PostRequest("User/register_user?", query, new StringContent(""));

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
            catch(HttpRequestException)
            {
                return true;
            }
            
        }
        
    }
    

}