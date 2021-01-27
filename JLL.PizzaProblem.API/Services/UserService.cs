using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using JLL.PizzaProblem.API.Models;
using AutoMapper;
using JLL.PizzaProblem.API.Data;
using Microsoft.EntityFrameworkCore;

namespace JLL.PizzaProblem.API.Services
{
    public class UserService : IUserService
    {
        // In memory list for now as storage
        private readonly PizzaProblemContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public UserService(IOptions<AppSettings> appSettings, IMapper mapper, PizzaProblemContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);

            // user was not found so return null
            if (user == null) return null;

            // authentication successful so generate response with new jwt token
            var createdAuthenticationResponse = _mapper.Map<AuthenticateResponse>(user);
            createdAuthenticationResponse.Token = GenerateJwtToken(user);

            return createdAuthenticationResponse;
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User GetById(int id)
        {
            return _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public User AddNewUser(User newUser)
        {
            newUser.Id = GetNewId();
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return _context.Users.FirstOrDefault(x => x.Id == newUser.Id);
        }

        public void UpdateUser(User userToUpdate)
        {
            if(_context.Users.AsNoTracking().FirstOrDefault(x => x.Id == userToUpdate.Id) != null)
            {
                _context.Users.Update(userToUpdate);
                _context.SaveChanges();
            }
        }

        public List<User> GetTopTenPizzaLove()
        {
            return _context.Users.OrderByDescending(i => i.PizzaLove).Take(10).ToList();
        }

        #region Private methods

        private int GetNewId()
        {
            return _context.Users.Count() + 1;
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

        #endregion
    }
}