using JLL.PizzaProblem.DataAccess.EF;
using JLL.PizzaProblem.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JLL.PizzaProblem.Services
{
    public class UserService : IUserService
    {
        // In memory list for now as storage
        private readonly PizzaProblemContext _context;

        public UserService(PizzaProblemContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == model.Username && x.Password == model.Password);

            // user was not found so return null
            if (user == null) return null;

            // authentication successful so generate response with new jwt token
            var createdAuthenticationResponse = new AuthenticateResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username
            };

            return createdAuthenticationResponse;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User> AddNewUserAsync(User newUser)
        {
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == newUser.Id);
        }

        public async Task<bool> UpdateUserAsync(User userToUpdate)
        {
            if(_context.Users.AsNoTracking().FirstOrDefault(x => x.Id == userToUpdate.Id) != null)
            {
                _context.Users.Update(userToUpdate);
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<List<User>> GetTopTenPizzaLoveAsync()
        {
            return await _context.Users.OrderByDescending(i => i.PizzaLove).Take(10).ToListAsync();
        }
    }
}