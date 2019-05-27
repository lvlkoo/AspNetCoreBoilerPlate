using System;

namespace Boilerplate.Models.Chat
{
    public class ChatMessageAttachmentModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string FileLink { get; set; }
        public long FileLenght { get; set; }
    }
}
