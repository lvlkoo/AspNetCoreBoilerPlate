using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Models.Auth
{
    public class SignUpModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
