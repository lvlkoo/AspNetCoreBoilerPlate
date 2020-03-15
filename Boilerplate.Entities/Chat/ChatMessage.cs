using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boilerplate.Entities.Chat
{
    public class ChatMessage: IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid SenderId { get; set; }
        public Guid ChannelId { get; set; }

        public ApplicationUser Sender { get; set; }
        public ChatChannel Channel { get; set; }

        public ICollection<ChatMessageAttachment> Attachments { get; set; }
    }
}
