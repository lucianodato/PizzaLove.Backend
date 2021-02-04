using System.Text.Json.Serialization;

namespace JLL.PizzaProblem.API.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public int PizzaLove { get; set; }
    }
}