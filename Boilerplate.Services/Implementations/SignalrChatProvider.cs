using System;
using System.Threading.Tasks;
using Boilerplate.Entities;
using Boilerplate.Entities.Chat;
using Boilerplate.Models.Chat.EventsData;
using Boilerplate.Models.Enums;
using Boilerplate.Services.Abstractions;
using Boilerplate.Services.SignalR;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Boilerplate.Services.Implementations
{
    public class SignalrChatProvider: IChatProvider
    {
        private readonly IHubContext<MainHub> _hubContext;
        private readonly IChatConnectionsStore _connectionsStore;

        public SignalrChatProvider(IHubContext<MainHub> hubContext, IChatConnectionsStore connectionsStore)
        {
            _hubContext = hubContext;
            _connectionsStore = connectionsStore;
        }

        public async Task UserConnectedToServer(ApplicationUser user, Guid channelId)
        {
            var eventData = new UserJoinedEventData
            {
                UserId = user.Id,
                Username = user.UserName
            };

            await SendEventToGroup(channelId, ChatEvent.UserConnected, eventData);
        }

        public async Task UserDisconnectedFromServer(ApplicationUser user, Guid channelId)
        {
            var eventData = new UserJoinedEventData
            {
                UserId = user.Id,
                Username = user.UserName
            };

            await SendEventToGroup(channelId, ChatEvent.UserDisconnected, eventData);
        }

        public async Task UserJoinedToChannel(ApplicationUser user, Guid channelId)
        {
            var userConnection = await _connectionsStore.GetUserConnection(user.Id);
            var eventData = new UserJoinedEventData
            {
                UserId = user.Id,
                Username = user.UserName
            };

            await SendEventToGroup(channelId, ChatEvent.UserJoined, eventData);

            if (userConnection != null)
                await _hubContext.Groups.AddToGroupAsync(userConnection, channelId.ToString());
        }

        public async Task UserLeftFromChannel(ApplicationUser user, Guid channelId)
        {
            var userConnection = await _connectionsStore.GetUserConnection(user.Id);
            var eventData = new UserJoinedEventData
            {
                UserId = user.Id,
                Username = user.UserName
            };

            await SendEventToGroup(channelId, ChatEvent.UserLeft, eventData);
            if (userConnection != null)
                await _hubContext.Groups.RemoveFromGroupAsync(userConnection, channelId.ToString());
        }

        public async Task BroadcastMessage(ApplicationUser sender, ChatMessage message)
        {
            var eventData = new BroadcastMessageEventData
            {
                MessageId = message.Id,
                Text = message.Message,
                UserId = sender.Id,
                Username = sender.UserName
            };

            await SendEventToGroup(message.ChannelId, ChatEvent.MessageReceived, eventData);
        }

        public async Task MessageEdited(ChatMessage message)
        {
            var eventData = new MessageEditedEventData
            {
                MessageId = message.Id
            };

            await SendEventToGroup(message.ChannelId, ChatEvent.MessageEdited, eventData);
        }

        public async Task MessageDeleted(ChatMessage message)
        {
            var eventData = new MessageEditedEventData
            {
                MessageId = message.Id
            };

            await SendEventToGroup(message.ChannelId, ChatEvent.MessageDeleted, eventData);
        }

        private string PrepareEventData<T>(T data)
        {
            var serialized = JsonConvert.SerializeObject(data);
            return serialized;
        }

        private async Task SendEventToGroup<T>(Guid groupId, ChatEvent chatEvent, T data) where T : ChatBaseEventData
        {
            data.EventCode = chatEvent;
            data.ChannelId = groupId;
            await _hubContext.Clients.Group(groupId.ToString()).SendAsync(chatEvent.ToString("G"), PrepareEventData(data));
        }
    }
}
