using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Boilerplate.DAL.Entities
{
    public class ApplicationRole: IdentityRole<Guid>, IEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Permissions { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
