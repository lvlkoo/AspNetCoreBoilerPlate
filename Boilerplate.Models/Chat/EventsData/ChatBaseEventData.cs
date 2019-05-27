using System;
using Boilerplate.Models.Enums;

namespace Boilerplate.Models.Chat.EventsData
{
    public class ChatBaseEventData
    {
        public ChatEvent EventCode { get; set; }
        public string EventName => EventCode.ToString("G");
        public Guid Channeld { get; set; }
    }
}
