using System;

namespace Boilerplate.Models.Chat.EventsData
{
    public class UserJoinedEventData: ChatBaseEventData
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
    }
}
