using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boilerplate.DAL.Entities.Chat
{
    public class ChatMessageAttachment: IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid MessageId { get; set; }
        public Guid UploadId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public FileUpload Upload { get; set; }
        public ChatMessage Message { get; set; }
    }
}
