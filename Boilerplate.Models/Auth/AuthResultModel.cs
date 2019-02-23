using System;

namespace Boilerplate.Models.Auth
{
    public class AuthResultModel
    {
        public string Token { get; set; }
        public DateTime Expire { get; set; }
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
