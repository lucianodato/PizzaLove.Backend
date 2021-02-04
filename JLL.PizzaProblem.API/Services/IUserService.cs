using System.Collections.Generic;
using System.Threading.Tasks;
using JLL.PizzaProblem.API.Models;

namespace JLL.PizzaProblem.API.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model);
        Task<List<User>> GetAllAsync();
        Task<User> AddNewUserAsync(User newUser);
        Task<User> GetByIdAsync(int id);
        Task<List<User>> GetTopTenPizzaLoveAsync();
        Task<bool> UpdateUserAsync(User userToUpdate);
    }
}