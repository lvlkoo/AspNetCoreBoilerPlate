using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Models.Auth
{
    public class SignInModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
