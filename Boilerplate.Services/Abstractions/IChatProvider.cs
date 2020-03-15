using System;
using System.Threading.Tasks;
using Boilerplate.Entities;
using Boilerplate.Entities.Chat;

namespace Boilerplate.Services.Abstractions
{
    public interface IChatProvider
    {
        Task UserConnectedToServer(ApplicationUser user, Guid channelId);
        Task UserDisconnectedFromServer(ApplicationUser user, Guid channelId);
        Task UserJoinedToChannel(ApplicationUser user, Guid channelId);
        Task UserLeftFromChannel(ApplicationUser user, Guid channelId);
        Task BroadcastMessage(ApplicationUser sender, ChatMessage message);
        Task MessageEdited(ChatMessage message);
        Task MessageDeleted(ChatMessage message);
    }
}
