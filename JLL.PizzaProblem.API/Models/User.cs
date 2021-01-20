using System.Text.Json.Serialization;

namespace JLL.PizzaProblem.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public int PizzaLove { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}