using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Boilerplate.DAL.Entities
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public string RefreshToken { get; set; }

        public ICollection<FileUpload> FileUploads { get; set; }
    }
}
