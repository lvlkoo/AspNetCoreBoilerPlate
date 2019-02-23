namespace Boilerplate.Models.Auth
{
    public class RefreshRequestModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
