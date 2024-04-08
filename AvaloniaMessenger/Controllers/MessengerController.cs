using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AvaloniaMessenger.Models;
using Newtonsoft.Json;

namespace AvaloniaMessenger.Controllers
{
    static class MessengerController
    {
        private static string _serverUrl { get; set; } = "https://localhost:7284";


        public static void SignIn(string login, string password)
        {
            
        }
        public static void SignUp(string login, string password, string username)
        {
            
        }
        private static T? MakeRequset<T>(string method, Dictionary<string, string> query)
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
                response = client.GetAsync(requestUri).Result;
            }
            catch
            {
                throw new HttpRequestException("Couldn't reach server");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.StatusCode.ToString());
            }
            if(response.Content.ToString() == null)
            {
                throw new Exception("Got null response");
            }

            var obj = JsonConvert.DeserializeObject<T>(response.Content.ToString());

            return obj;
        }
    }
}
