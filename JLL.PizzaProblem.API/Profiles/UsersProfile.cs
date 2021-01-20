using AutoMapper;
using JLL.PizzaProblem.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JLL.PizzaProblem.API.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<User, UserForCreation>().ReverseMap();
            CreateMap<AuthenticateResponse, User>().ReverseMap();
        }
    }
}
