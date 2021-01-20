using System.Collections.Generic;
using JLL.PizzaProblem.Models;

namespace JLL.PizzaProblem.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        List<User> GetAll();
        User AddNewUser(User newUser);
        User GetById(int id);
    }
}