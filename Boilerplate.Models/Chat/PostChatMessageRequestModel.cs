using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Boilerplate.Models.Validators;

namespace Boilerplate.Models.Chat
{
    public class PostChatMessageRequestModel
    {
        [ValidGuid]
        public Guid Channeld { get; set; }
        [Required]
        public string Text { get; set; }

        public List<Guid> Attachments { get; set; }
    }
}
