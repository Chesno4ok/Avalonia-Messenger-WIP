using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaMessenger.Controllers
{
    class ApiCaller
    {
        public Uri ServerUrl { get; set; }

        public ApiCaller(Uri serverUrl)
        {
            ServerUrl = serverUrl;
        }
        public T? GetRequest<T>(string method, string query)
        {
            var client = new HttpClient();
            string requestUri = ServerUrl.ToString() + method + query;

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
                throw new HttpRequestException("Bad Request", null, response.StatusCode);
            }
            if (response.Content.ToString() == null)
            {
                throw new Exception("Got null response");
            }

            var obj = JsonConvert.DeserializeObject<T>(response.Content.ToString());

            return obj;
        }

        public HttpResponseMessage GetRequest(string method, string query)
        {
            var client = new HttpClient();
            string requestUri = ServerUrl.ToString() + method + query;

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
                throw new HttpRequestException("Bad Request", null, response.StatusCode);
            }
            if (response.Content.ToString() == null)
            {
                throw new Exception("Got null response");
            }

            return response;
        }
        public T? PostRequest<T>(string method, string query, HttpContent content)
        {
            var client = new HttpClient();
            string requestUri = ServerUrl.ToString() + method + query;

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = client.PostAsync(requestUri, content).Result;
            }
            catch
            {
                throw new HttpRequestException("Couldn't reach server");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Bad Request", null, response.StatusCode);
            }
            if (response.Content.ToString() == null)
            {
                throw new Exception("Got null response");
            }

            var obj = JsonConvert.DeserializeObject<T>(response.Content.ToString());

            return obj;
        }

        public HttpResponseMessage PostRequest(string method, string query, HttpContent content)
        {
            var client = new HttpClient();
            string requestUri = ServerUrl.ToString() + method + query;

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = client.PostAsync(requestUri, content).Result;
            }
            catch
            {
                throw new HttpRequestException("Couldn't reach server");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Bad Request", null, response.StatusCode);
            }
            if (response.Content.ToString() == null)
            {
                throw new Exception("Got null response");
            }

            return response;
        }
    }   
}