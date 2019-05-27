using System;

namespace Boilerplate.Models.Chat.EventsData
{
    public class MessageEditedEventData: ChatBaseEventData
    {
        public Guid MessageId { get; set; }
    }
}
