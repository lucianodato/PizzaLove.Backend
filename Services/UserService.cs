using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using JLL.PizzaProblem.Models;

namespace JLL.PizzaProblem.Services
{
    public class UserService : IUserService
    {
        // In memory list for now as storage
        private List<User> _users = new List<User>
        {
            new User { Id = 1, FirstName = "Test", LastName = "Test", Username = "test", Password = "test" },
            new User { Id = 2, FirstName = "User", LastName = "User", Username = "user", Password = "user" }
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);

            // user was not found so return null
            if (user == null) return null;

            // authentication successful so generate jwt token
            var token = GenerateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        public User GetById(int id)
        {
            return _users.Find(x => x.Id == id);
        }

        private int GetNewId()
        {
            return _users.Count + 1;
        }

        public User AddNewUser(User newUser)
        {
            newUser.Id = GetNewId();
            _users.Add(newUser);
            return _users.Find(x => x.Id == newUser.Id);
        }

        private string GenerateJwtToken(User user)
        {
            // generate token that is valid for 1 days. 
            // TODO this should be done using a refreshing mechanism instead
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}