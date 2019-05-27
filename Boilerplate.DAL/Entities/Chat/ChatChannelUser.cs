using System;

namespace Boilerplate.DAL.Entities.Chat
{
    public class ChatChannelUser
    {
        public Guid Channeld { get; set; }
        public Guid UserId { get; set; }

        public ChatChannel Channel { get; set; }
        public ApplicationUser User { get; set; }
    }
}
