using System.ComponentModel.DataAnnotations;

namespace JLL.PizzaProblem.API.Dtos
{
    public class AuthenticateRequestDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}