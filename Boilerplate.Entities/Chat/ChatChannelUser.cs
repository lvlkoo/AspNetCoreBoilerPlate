using System;

namespace Boilerplate.Entities.Chat
{
    public class ChatChannelUser
    {
        public Guid ChannelId { get; set; }
        public Guid UserId { get; set; }

        public ChatChannel Channel { get; set; }
        public ApplicationUser User { get; set; }
    }
}
