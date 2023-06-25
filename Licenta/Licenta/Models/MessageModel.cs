using System;
using System.Collections.Generic;
using System.Text;

namespace Licenta.Models
{
    public class MessageModel
    {
        public string Message { get; set; }
        public MessageSender Sender { get; set; }

        public string Color { get; set; }

        public MessageModel(string query, MessageSender sender, string color)
        {
            this.Message = query;
            this.Sender = sender;
            this.Color = color;
        }
    }
    public enum MessageSender
    {
        User,
        Bot
    }
}
