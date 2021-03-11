using AutoMapper;
using Boilerplate.Entities.Chat;
using Boilerplate.Models.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Boilerplate.Api.Utils
{
    /// <summary>
    /// Provide mapping for chat attachment link
    /// </summary>
    public class UploadLinkMapperValueResolver: IValueResolver<ChatMessageAttachment, ChatMessageAttachmentModel, string>
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _contextAccessor;

        /// <inheritdoc />
        public UploadLinkMapperValueResolver(LinkGenerator linkGenerator, IHttpContextAccessor contextAccessor)
        {
            _linkGenerator = linkGenerator;
            _contextAccessor = contextAccessor;
        }

        /// <inheritdoc />
        public string Resolve(ChatMessageAttachment source, ChatMessageAttachmentModel destination, string destMember,
            ResolutionContext context)
        {
            var requrestApiVersion = _contextAccessor.HttpContext.GetRequestedApiVersion();
            var version = requrestApiVersion.MajorVersion;
            var url = _linkGenerator.GetPathByAction(_contextAccessor.HttpContext, "Get", $"UploadsV{version}",
                new { version = version.ToString(), id = source.Upload.Id.ToString()});
            return url;
        }
    }
}
