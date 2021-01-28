using JLL.PizzaProblem.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JLL.PizzaProblem.Services
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