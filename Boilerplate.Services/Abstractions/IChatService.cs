using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boilerplate.Models;
using Boilerplate.Models.Chat;

namespace Boilerplate.Services.Abstractions
{
    public interface IChatService
    {
        Task<IEnumerable<UserChannelInfoModel>> GetUserChannels();
        Task<IEnumerable<UserChannelInfoModel>> GetUserChannels(Guid userId);
        Task<IEnumerable<ChatMessageModel>> GetChannelMessages(Guid channelId);
        Task<JoinChannelResultModel> JoinToGroupChannel(JoinChannelRequestModel model);
        Task<JoinChannelResultModel> JoinToPrivateChannel(JoinChannelRequestModel model);
        Task LeaveChannel(Guid channeld);
        Task<ChatMessageModel> GetMessage(Guid id);
        Task<ChatMessageModel> PostMessage(PostChatMessageRequestModel model);
        Task<ChatMessageModel> EditMessage(Guid id, PostChatMessageRequestModel model);
        Task DeleteMessage(Guid id);
    }
}
