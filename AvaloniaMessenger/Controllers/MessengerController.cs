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

        public ApiCaller apiCaller { get; set; }
        private QueryStringBuilder queryBuilder { get; set; }

        

        public MessengerController(Uri serverInfo)
        {
            apiCaller = new ApiCaller(serverInfo);

            queryBuilder = new QueryStringBuilder();
        }

        #region MessageController
        [QueryInfo("userId", "token", "chatId", "amount")]
        public Message[]? GetLastMessages(int userId, string token, int chatId, int amount)
        {
            var query = queryBuilder.GetQueryString(userId, token, chatId, amount);

            try
            {
                return apiCaller.GetRequest<Message[]>("Message/get_last_messages?", query);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        [QueryInfo("userId", "token", "chatId","messageId", "amount")]
        public Message[]? GetPreviousMessages(int userId, string token, int chatId, int messageId, int amount)
        {
            var query = queryBuilder.GetQueryString(userId, token, chatId, messageId, amount);

            try
            {
                return apiCaller.GetRequest<Message[]>("Message/get_previous_messages?", query);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        #endregion

        #region UserController
        [QueryInfo("login", "password")]
        public User? SignIn(string login, string password)
        {
            User? user = new User() { Login = login, Password = password };

            var query = queryBuilder.GetQueryString(login, password);

            user = apiCaller.GetRequest<User>("User/get_token?", query);

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
            catch (HttpRequestException)
            {
                return true;
            }

        }

        [QueryInfo("userId")]
        public User? GetUser(int userId)
        {
            var query = queryBuilder.GetQueryString(userId);

            try
            {
                return apiCaller.GetRequest<User>("User/get_user?", query);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        [QueryInfo("username")]
        public User[]? SearchUser(string username)
        {
            var query = queryBuilder.GetQueryString(username);

            try
            {
                return apiCaller.GetRequest<User[]>("User/search_user?", query);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        #endregion

        #region ChatController
        [QueryInfo("userId", "token")]
        public Chat[]? GetChats(string userId, string token)
        {
            var query = queryBuilder.GetQueryString(userId, token);

            try
            {
                return apiCaller.GetRequest<Chat[]>("Chat/get_chats?", query);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        [QueryInfo("userId", "token", "chatId")]
        public User[]? GetChatUsers(int userId, string token, int chatId)
        {
            var query = queryBuilder.GetQueryString(userId, token, chatId);

            try
            {
                return apiCaller.GetRequest<User[]>("Chat/get_users?", query);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        [QueryInfo("userId", "token", "chatName")]
        public bool CreateChat(int userId, string token, string chatName, HttpContent content)
        {
            content.Headers.Clear();

            var query = queryBuilder.GetQueryString(userId, token, chatName);

            content.Headers.Add("Content-Type", "application/json");

            try
            {
                var message = apiCaller.PostRequest("Chat/create_chat?", query, content);

                return message.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        #endregion







    }
    

}