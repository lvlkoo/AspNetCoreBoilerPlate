using System;

namespace Boilerplate.Models.Chat.EventsData
{
    public class BroadcastMessageEventData: ChatBaseEventData
    {
        public Guid MessageId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
    }
}
