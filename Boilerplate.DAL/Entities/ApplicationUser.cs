using System;
using System.Collections.Generic;
using Boilerplate.DAL.Entities.Chat;
using Microsoft.AspNetCore.Identity;

namespace Boilerplate.DAL.Entities
{
    public class ApplicationUser: IdentityUser<Guid>, IEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }  
        public string RefreshToken { get; set; }

        public virtual ICollection<FileUpload> FileUploads { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<ChatChannelUser> ChatUsers { get; set; }
    }
}
