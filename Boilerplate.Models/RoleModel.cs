using System;
using System.Collections.Generic;

namespace Boilerplate.Models
{
    public class RoleModel : IModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Permissions { get; set; }
    }
}
