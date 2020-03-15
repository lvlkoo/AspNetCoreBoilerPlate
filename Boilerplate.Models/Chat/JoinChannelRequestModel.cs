using System;
using Boilerplate.Commons.Validators;

namespace Boilerplate.Models.Chat
{
    public class JoinChannelRequestModel
    {
        /// <summary>
        /// Group or user id
        /// </summary>
        [ValidGuid]
        public Guid ChannelId { get; set; }
    }
}
