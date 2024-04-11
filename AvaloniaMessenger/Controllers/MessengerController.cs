using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
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
            var query = GetDictionary( login, password);

            User? user = new User();

            user = GetRequest<User>("/User/get_token", query, null);

            return user;
        }
        public static User SignUp(string login, string password, string name)
        {
            var query = GetDictionary(name, login, password);

            User? user = new User();
            try
            {
                user = GetRequest<User>("/User/register_user", query, new StringContent(""));
            }
            catch
            {

            }

            return user;
        }
        private static T? GetRequest<T>(string method, Dictionary<string, string> query, HttpContent? body)
        {
            var client = new HttpClient();
            string requestUri = _serverUrl + method;

            foreach(var p in query)
            {
                requestUri += $"{p.Key}={p.Value}&";
            }

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = body == null ?
                client.GetAsync(requestUri).Result :
                client.PostAsync(requestUri, body).Result;
            }
            catch
            {
                throw new HttpRequestException("Couldn't reach server");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Couldn't make a request", null, response.StatusCode);
            }
            if(response.Content.ToString() == null)
            {
                throw new Exception("Got null response");
            }

            var obj = JsonConvert.DeserializeObject<T>(response.Content.ToString());

            return obj;
        }

        private static Dictionary<string, string> GetDictionary(params string[] query)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();

            foreach(var i in query)
            {
                res.Add(nameof(i), i);
            }

            return res;
        }
    }
}