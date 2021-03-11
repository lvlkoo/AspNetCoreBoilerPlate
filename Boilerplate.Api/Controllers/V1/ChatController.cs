using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boilerplate.Api.ActionFilters;
using Boilerplate.Commons.Attributes;
using Boilerplate.Commons.Static;
using Boilerplate.Models;
using Boilerplate.Models.Chat;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Api.Controllers.V1
{
    /// <summary>
    /// Chat endpoints
    /// </summary>
    [Authorize, ValidatesPermissions]
    [ApiVersion("1.0"), Route("api/v{version:apiVersion}/chat")]
    public class ChatController : BaseApiController
    {
        private readonly IChatService _chatService;

        /// <inheritdoc />
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Get user's channels
        /// </summary>
        [HttpGet("channels"), PermissionRequired(Permissions.View)]
        public async Task<BaseResponse<IEnumerable<UserChannelInfoModel>>> Get() =>
            await PrepareResponse(_chatService.GetUserChannels);

        /// <summary>
        /// Get messages in channel
        /// </summary>
        [HttpGet("channels/{id}"), PermissionRequired(Permissions.View)]
        public async Task<BaseResponse<IEnumerable<ChatMessageModel>>> Get(Guid id) =>
            await PrepareResponse(_chatService.GetChannelMessages, id);

        /// <summary>
        /// Post message to channel
        /// </summary>
        [HttpPost("messages"), PermissionRequired(Permissions.Edit)]
        public async Task<BaseResponse<ChatMessageModel>> Post(PostChatMessageRequestModel model) =>
            await PrepareResponse(_chatService.PostMessage, model);

        /// <summary>
        /// Edit message by id
        /// </summary>
        [HttpPut("messages/{id}"), PermissionRequired(Permissions.Edit)]
        public async Task<BaseResponse<ChatMessageModel>> Put(Guid id, PostChatMessageRequestModel model) =>
            await PrepareResponse(_chatService.EditMessage, id, model);

        /// <summary>
        /// Delete message by id
        /// </summary>
        [HttpDelete("messages/{id}"), PermissionRequired(Permissions.Delete)]
        public async Task<BaseResponse> Delete(Guid id) =>
            await PrepareResponse(_chatService.DeleteMessage, id);

        /// <summary>
        /// Join channel by id
        /// </summary>
        [HttpPost("join/group"), PermissionRequired(Permissions.Edit)]
        public async Task<BaseResponse<JoinChannelResultModel>> JoinGroup(JoinChannelRequestModel model) =>
            await PrepareResponse(_chatService.JoinToGroupChannel, model);

        /// <summary>
        /// Join private channel by user Id
        /// </summary>
        [HttpPost("join/private"), PermissionRequired(Permissions.Edit)]
        public async Task<BaseResponse<JoinChannelResultModel>> JoinPrivate(JoinChannelRequestModel model) =>
            await PrepareResponse(_chatService.JoinToPrivateChannel, model);
    }
}
