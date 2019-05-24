using System;

namespace Boilerplate.Models
{
    public class UserModel : IModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
    }
}