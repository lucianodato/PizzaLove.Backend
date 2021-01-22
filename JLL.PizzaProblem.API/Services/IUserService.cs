using System.Collections.Generic;
using JLL.PizzaProblem.API.Models;

namespace JLL.PizzaProblem.API.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        List<User> GetAll();
        User AddNewUser(User newUser);
        User GetById(int id);
        List<User> GetTopTenPizzaLove();
        void UpdateUser(User userToUpdate);
    }
}