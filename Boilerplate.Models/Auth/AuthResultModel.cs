using System;
using System.Collections.Generic;

namespace Boilerplate.Models.Auth
{
    public class AuthResultModel
    {
        public string Token { get; set; }
        public DateTime Expire { get; set; }
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; }
        public List<string> Permissions { get; set; }
    }
}
