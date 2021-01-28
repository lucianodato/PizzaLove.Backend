using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JLL.PizzaProblem.API.Dtos
{
    public class UserForPatchDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int PizzaLove { get; set; }
    }
}