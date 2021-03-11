using System;
using System.Collections.Generic;
using AutoMapper;
using Boilerplate.Entities;
using Boilerplate.Entities.Chat;
using Boilerplate.Models;
using Boilerplate.Models.Chat;

namespace Boilerplate.Api.Utils
{
    /// <summary>
    /// Root auto mapper settings
    /// </summary>
    public class AutomapperProfile: Profile
    {
        /// <inheritdoc />
        public AutomapperProfile()
        {
            CreateMap<ApplicationRole, RoleModel>()
                .ForMember(dist => dist.Permissions, opt => opt.MapFrom(r => new List<string>(r.Permissions.Split(",", StringSplitOptions.None))));

            CreateMap<ChatMessage, ChatMessageModel>();
            CreateMap<ChatMessageAttachment, ChatMessageAttachmentModel>()
                .ForMember(dist => dist.ContentType, opt => opt.MapFrom(r => r.Upload.ContentType))
                .ForMember(dist => dist.FileLenght, opt => opt.MapFrom(r => r.Upload.FileLength))
                .ForMember(dist => dist.FileName, opt => opt.MapFrom(r => r.Upload.FileName))
                .ForMember(dist => dist.FileLink, opt => opt.MapFrom<UploadLinkMapperValueResolver>());
        }
    }
}
