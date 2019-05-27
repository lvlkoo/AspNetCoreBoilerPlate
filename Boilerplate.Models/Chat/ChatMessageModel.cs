using System;
using System.Collections.Generic;

namespace Boilerplate.Models.Chat
{
    public class ChatMessageModel
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid SenderId { get; set; }
        public Guid Channeld { get; set; }

        public List<ChatMessageAttachmentModel> Attachments { get; set; }
    }
}
