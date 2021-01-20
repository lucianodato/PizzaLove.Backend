using System.ComponentModel.DataAnnotations;

namespace JLL.PizzaProblem.API.Models
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}