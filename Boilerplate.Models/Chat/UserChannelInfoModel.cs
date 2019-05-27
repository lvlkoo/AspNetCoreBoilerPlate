using System;

namespace Boilerplate.Models.Chat
{
    public class UserChannelInfoModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public ChatMessageModel LastMessage { get; set; }
    }
}
