using AvaloniaMessenger.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaMessenger.ViewModels;

namespace AvaloniaMessenger.Controllers
{
    class WebSocketMessageReceiver
    {
        WebSocketController wsController;


        public WebSocketMessageReceiver(WebSocketController wsController)
        {
            this.wsController = wsController;
        }
        
        [TableNotification("Messages", "INSERT")]
        public void ProccessMessage(string notificationJson)
        {
            var notification = JsonConvert.DeserializeObject<DbNotification<Message>>(notificationJson);

            if (notification == null)
                return;

            if (wsController.LoadNewMessagesCommand != null)
                wsController.LoadNewMessagesCommand.Execute(notification.data).Subscribe();
        }
        [TableNotification("ChatUsers", "INSERT")]
        public void ProccessChatInvitation(string notificationJson)
        {
            var notification = JsonConvert.DeserializeObject<DbNotification<ChatUser>>(notificationJson);

            if (notification == null)
                return;

            if (wsController.LoadNewChatCommand != null)
                wsController.LoadNewChatCommand.Execute(notification.data).Subscribe();
        }
        [TableNotification("Chats", "DELETE")]
        public void RemoveChat(string notificationJson)
        {
            var notification = JsonConvert.DeserializeObject<DbNotification<Chat>>(notificationJson);

            if (notification == null)
                return;

            if (wsController.DeleteChatCommand != null)
                wsController.DeleteChatCommand.Execute(notification.data).Subscribe();
        }
        [TableNotification("ChatUsers", "DELETE")]
        public void RemoveChatUser(string notificationJson)
        {
            var notification = JsonConvert.DeserializeObject<DbNotification<ChatUser>>(notificationJson);
            var chat = new Chat()
            {
                Id = notification.data.ChatId
            };

            if (wsController.DeleteChatCommand != null)
                wsController.DeleteChatCommand.Execute(chat).Subscribe();
        }
    }
    class TableNotification : Attribute
    {
        public string TableName;
        public string Action;
        public TableNotification(string tableName, string action)
        {
            Action = action;
            TableName = tableName;
        }
    }
}
