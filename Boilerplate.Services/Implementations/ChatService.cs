using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Boilerplate.DAL;
using Boilerplate.DAL.Entities.Chat;
using Boilerplate.Models;
using Boilerplate.Models.Chat;
using Boilerplate.Models.Exceptions;
using Boilerplate.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Services.Implementations
{
    public class ChatService: BaseDataService, IChatService
    {
        private readonly IAuthService _authService;
        private readonly IChatProvider _chatProvider;

        public ChatService(IAuthService authService, IChatProvider chatProvider, ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _authService = authService;
            _chatProvider = chatProvider;
        }

        public Task<IEnumerable<UserChannelInfoModel>> GetUserChannels()
        {
            return GetUserChannels(_authService.GetAuthorizedUserId());
        }

        public async Task<IEnumerable<UserChannelInfoModel>> GetUserChannels(Guid userId)
        {
            var query =
                from channel in DbContext.ChatChannels
                join channelUser in DbContext.ChatChannelUsers on channel.Id equals channelUser.Channeld into channelUsers
                where channelUsers.Select(_ => _.UserId).Contains(userId)
                select new UserChannelInfoModel
                {
                    Id = channel.Id
                };

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<ChatMessageModel>> GetChannelMessages(Guid channelId)
        {
            var messages = await DbContext.ChatMessages
                .Include(cm => cm.Attachments)
                .ThenInclude(a => a.Upload)
                .Where(cm => cm.Channeld == channelId)
                .ToListAsync();
            return MapList<ChatMessageModel>(messages);
        }

        public async Task<JoinChannelResultModel> JoinToGroupChannel(JoinChannelRequestModel model)
        {
            var user = await _authService.GetAuthorizedUser();

            var channel = await DbContext.ChatChannels.FindAsync(model.ChannelId);
            if (channel == null)
            {
                channel = new ChatChannel();

                await DbContext.AddAsync(channel);
                await DbContext.SaveChangesAsync();
            }

            var channelUser = new ChatChannelUser
            {
                UserId = user.Id,
                Channeld = channel.Id
            };

            await DbContext.AddAsync(channelUser);
            await DbContext.SaveChangesAsync();

            await _chatProvider.UserJoinedToChannel(user, channel.Id);

            return new JoinChannelResultModel { Channeld = channel.Id };
        }

        public async Task<JoinChannelResultModel> JoinToPrivateChannel(JoinChannelRequestModel model)
        {
            var currentUser = await _authService.GetAuthorizedUser();

            var user = await DbContext.Users.FindAsync(model.ChannelId);
            if (user == null)
                throw new EntityNotFoundException();

            var channel = new ChatChannel();

            await DbContext.AddAsync(channel);
            await DbContext.SaveChangesAsync();

            var receiver = new ChatChannelUser
            {
                UserId = model.ChannelId,
                Channeld = channel.Id
            };

            var sender = new ChatChannelUser
            {
                UserId = currentUser.Id,
                Channeld = channel.Id
            };

            await DbContext.AddAsync(receiver);
            await DbContext.AddAsync(sender);

            await DbContext.SaveChangesAsync();

            //creating private chnannel with receiver 1st
            await _chatProvider.UserJoinedToChannel(user, channel.Id);
            //adding sender to this private chnannel
            await _chatProvider.UserJoinedToChannel(currentUser, channel.Id);

            return new JoinChannelResultModel {Channeld = channel.Id};
        }

        public async Task LeaveChannel(Guid channeld)
        {
            var currentUserId = _authService.GetAuthorizedUserId();
            var channelUser = await DbContext.ChatChannelUsers
                .Include(cu => cu.User)
                .FirstOrDefaultAsync(cu => cu.Channeld == channeld && cu.UserId == currentUserId);

            if (channelUser == null)
                throw new EntityNotFoundException("User doesn't belong to this group");

            DbContext.ChatChannelUsers.Remove(channelUser);
            await DbContext.SaveChangesAsync();

            await _chatProvider.UserLeftFromChannel(channelUser.User, channeld);
        }

        public async Task<ChatMessageModel> GetMessage(Guid id)
        {
            var message = await DbContext.ChatMessages
                .Include(m => m.Attachments)
                .ThenInclude(a => a.Upload)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
                throw new EntityNotFoundException("Message with specified id is not found");

            return Map<ChatMessageModel>(message);
        }

        public async Task<ChatMessageModel> PostMessage(PostChatMessageRequestModel model)
        {
            var currentUser = await _authService.GetAuthorizedUser();

            var channel = await DbContext.ChatChannels.FindAsync(model.Channeld);
            if (channel == null)
                throw new EntityNotFoundException("Channel with specified id is not found");

            var dbAttachmentsCount = await DbContext.FileUploads
                .CountAsync(_ => model.Attachments.Contains(_.Id));
            if (dbAttachmentsCount < model.Attachments.Count)
                throw new EntityNotFoundException("One of the specified attachment id is not found");

            var message = new ChatMessage
            {
                Channeld = channel.Id,
                SenderId = currentUser.Id,
                Message = model.Text,
                Attachments = model.Attachments.Select(id => new ChatMessageAttachment
                {
                    UploadId = id
                }).ToList(),
            };

            await DbContext.AddAsync(message);
            await DbContext.SaveChangesAsync();

            await _chatProvider.BroadcastMessage(currentUser, message);

            return await GetMessage(message.Id);
        }

        public async Task<ChatMessageModel> EditMessage(Guid id, PostChatMessageRequestModel model)
        {
            var message = await DbContext.ChatMessages.FindAsync(id);
            if (message == null)
                throw new EntityNotFoundException("Message with specified id is not found");

            message.Message = model.Text;

            await DbContext.SaveChangesAsync();

            return await GetMessage(id);
        }

        public async Task DeleteMessage(Guid id)
        {
            var message = await DbContext.ChatMessages.FindAsync(id);
            if (message == null)
                throw new EntityNotFoundException("Message with specified id is not found");

            DbContext.Remove(message);
            await DbContext.SaveChangesAsync();
        }
    }
}
