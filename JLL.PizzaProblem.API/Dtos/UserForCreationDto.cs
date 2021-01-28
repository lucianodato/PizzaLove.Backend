using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JLL.PizzaProblem.API.Dtos
{
    public class UserForCreationDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int PizzaLove { get; set; } = 0;
    }
}